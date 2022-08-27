import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { Counter, Trend } from 'k6/metrics';

// Parameters & Constants
const USERNAME =  `${randomString(10)}@example.com`;
const PASSWORD =  'fakepassword'; 
const BASE_URL = 'https://test-api.k6.io';
const DEBUG = true;

// Counters
var ReadCounter = new Counter('ReadCounter');

// Trends
var ReadTrend = new Trend('ReadTrend');

const ExecutionType = 
{
    smoke:          'smoke', 
    load:           'load', 
    stress:         'stress', 
    soak:           'soak', 
    spike:          'spike', 
    performance:    'performance' //This is my custom performance that that suits my performance testing requirements in a development stage
}

var ExecutionScenarios;
//To execute a different test scenario: change the Exectution variable to one of the ExecutionTypes that are specified above
var Execution = 'performance';

switch(Execution)
{
	case ExecutionType.smoke:
		ExecutionScenarios = 
        {
			ReadScenario: 
            {
                exec: 'ReadTests',
                executor: 'ramping-arrival-rate', 
                startTime: '0s', 
                startRate: 1, 
                preAllocatedVUs: 4, 
                stages: 
                [
                    { duration: '30s', target: 1},
                    { duration: '30s', target: 2}
                ]
            },            
            CreateUpdateDeleteScenario: 
            {
                exec: 'CreateUpdateDeleteTests',
                executor: 'ramping-arrival-rate',
                startTime: '0s',
                startRate: 2,
                preAllocatedVUs: 8,
                stages: 
                [
                    { duration: '30s', target: 2},
                    { duration: '30s', target: 4}
                ]
            }
        }; 
        break;
    case ExecutionType.load:
        ExecutionScenarios = 
        {
            ReadScenario: 
            {
                exec: 'ReadTests',
                executor: 'ramping-arrival-rate',
                startTime: '0s',
                startRate: 1,
                preAllocatedVUs: 4,
                stages: 
                [
                    { duration: '30s', target: 2},
                    { duration: '45m', target: 2}
                ]
            },            
            CreateUpdateDeleteScenario: 
            {
                exec: 'CreateUpdateDeleteTests',
                executor: 'ramping-arrival-rate',
                startTime: '0s',
                startRate: 2,
                preAllocatedVUs: 8,
                stages: 
                [
                    { duration: '30s', target: 4},
                    { duration: '45m', target: 4}
                ]
            } 
        }; 
        break;  
    case ExecutionType.stress:
        ExecutionScenarios = 
        {
            ReadScenario: 
            {
                exec: 'ReadTests',
                executor: 'ramping-arrival-rate',
                startTime: '0s',
                startRate: 10,
                preAllocatedVUs: 160,
                stages: 
                [
                    { duration: '5m', target: 10},
                    { duration: '5m', target: 20},
                    { duration: '5m', target: 40},
                    { duration: '5m', target: 80}
                ]
            },            
            CreateUpdateDeleteScenario: 
            {
                exec: 'CreateUpdateDeleteTests',
                executor: 'ramping-arrival-rate',
                startTime: '0s',
                startRate: 10,
                preAllocatedVUs: 160,
                stages: 
                [
                    { duration: '5m', target: 10},
                    { duration: '5m', target: 20},
                    { duration: '5m', target: 40},
                    { duration: '5m', target: 80}
                ]
            }
        }; 
        break;  
    case ExecutionType.soak:
        ExecutionScenarios = 
        {
            ReadScenario: 
            {
                exec: 'ReadTests',
                executor: 'ramping-arrival-rate',
                startTime: '0s',
                startRate: 1,
                preAllocatedVUs: 40,
                stages: 
                [
                    { duration: '5m', target: 5},
                    { duration: '5m', target: 10},
                    { duration: '8h', target: 20}
                ]
            },            
            CreateUpdateDeleteScenario: 
            {
                exec: 'CreateUpdateDeleteTests',
                executor: 'ramping-arrival-rate',
                startTime: '0s',
                startRate: 1,
                preAllocatedVUs: 40,
                stages: 
                [
                    { duration: '5m', target: 5},
                    { duration: '5m', target: 10},
                    { duration: '8h', target: 20}
                ]
            }
        }; 
        break;  
    case ExecutionType.spike:
        ExecutionScenarios = 
        {
            ReadScenario: 
            {
                exec: 'ReadTests',
                executor: 'ramping-arrival-rate',
                startTime: '0s',
                startRate: 1,
                preAllocatedVUs: 2000,
                stages: 
                [
                    { duration: '10s', target: 100},
                    { duration: '1m', target: 100},
                    { duration: '10s', target: 1400},
                    { duration: '3m', target: 1400},
                    { duration: '10s', target: 100},
                    { duration: '3m', target: 100},
                    { duration: '10s', target: 0}
                ]
            }
        }; 
        break;           
    case ExecutionType.performance:
        ExecutionScenarios = 
        {
            ReadScenario: 
            {
                exec: 'ReadTests',
                executor: 'ramping-arrival-rate',
                startTime: '0s',
                startRate: 1,
                preAllocatedVUs: 20,
                stages: 
                [
                    { duration: '10s', target: 10},
                    { duration: '1m', target: 10},
                    { duration: '10s', target: 0}
                ]
            },
            CreateUpdateDeleteScenario: 
            {
                exec: 'CreateUpdateDeleteTests',
                executor: 'ramping-arrival-rate',
                startTime: '0s',
                startRate: 1,
                preAllocatedVUs: 20,
                stages: 
                [
                    { duration: '10s', target: 10},
                    { duration: '1m', target: 10},
                    { duration: '10s', target: 0}
                ]
            }
        }
        break;        
}

export let options =
{
    scenarios: ExecutionScenarios,
    thresholds: 
    {
        http_req_failed: ['rate<0.05'],   
        'http_req_duration': ['p(95)<500', 'p(99)<1500'],
        'http_req_duration{name:Read}': ['avg<500', 'max<1000'],
        'http_req_duration{name:Create}': ['avg<600', 'max<1000'],
        'http_req_duration{name:Update}': ['avg<400'],
        'http_req_duration{name:Delete}': ['avg<400']
    }
};  

function randomString(length) 
{
    const charset = 'abcdefghijklmnopqrstuvwxyz';
    let res = '';
    while (length--) res += charset[Math.random() * charset.length | 0];
    return res;
}

function getRandom(min, max) 
{
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
}  

function formatDate(date) 
{
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0'+ minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return (date.getMonth()+1) + "/" + date.getDate() + "/" + date.getFullYear() + "  " + strTime;
}

function logInDebugMode(textToLog)
{
    if (DEBUG)
    {
        console.log(`${textToLog}`); 
    }
}

// Testing WebApi reading data
export function ReadTests(authToken)
{
    var idCroc = getRandom(1, 8);

    let response = http.get
    (
        `${BASE_URL}/public/crocodiles/1/`,
        { tags: { name: 'Read' } }
    );

    const isSuccessfulRequest = check(response, 
    {
        "Document request succeed": () => response.status == 200 //Ok
    });

    if (isSuccessfulRequest)
    {
        logInDebugMode(`response.body: ${response.body}`)
        ReadTrend.add(response.timings.duration);
        ReadCounter.add(1);
        let body = JSON.parse(response.body);
        var age = body.age; 

        check(age, 
        {
            'Crocs are older than 5 years of age': Math.min(age) > 5
        });
    }  
}    

// Testing WebApi creating, updating and deleting data
export function CreateUpdateDeleteTests(authToken)
{
  const requestConfigWithTag = tag => (
    {
        headers: 
        {
            Authorization: `Bearer ${authToken}`
        },
        tags: Object.assign({}, 
        {
            name: 'PrivateCrocs' //Additional tag
        }, tag)
    });

  group('Create and modify crocs', () => 
  {
    let URL = `${BASE_URL}/my/crocodiles/`;

    group('Create crocs', () => 
    {
        const payload = 
        {
            name: `Name ${randomString(10)}`,
            sex: 'M',
            date_of_birth: '2001-01-01',
        };

        const createResponse = http.post(URL, payload, requestConfigWithTag({ name: 'Create' }));

        if (check(createResponse, { 'Croc created correctly': (r) => r.status === 201 })) 
        {
            URL = `${URL}${createResponse.json('id')}/`;
        } 
        else 
        {
            logInDebugMode(`Unable to create a Croc ${createResponse.status} ${createResponse.body}`);
            return;
        }
    });

    group('Update croc', () => 
    {
        const payload = { name: 'New name' };

        const updateResponse = http.patch(URL, payload, requestConfigWithTag({ name: 'Update' }));

        const isSuccessfulUpdate = check(updateResponse, 
        {
            'Update worked': () => updateResponse.status === 200,
            'Updated name is correct': () => updateResponse.json('name') === 'New name',
        });

        if (!isSuccessfulUpdate) 
        {
            logInDebugMode(`Unable to update the croc ${updateResponse.status} ${updateResponse.body}`);
            return;
        }
    });

    const deleteResponse = http.del(URL, null, requestConfigWithTag({ name: 'Delete' }));

    const isSuccessfulDelete = check(null, 
    {
        'Croc was deleted correctly': () => deleteResponse.status === 204, //NoContent
    });

    if (!isSuccessfulDelete) 
    {
        logInDebugMode(`Croc was not deleted properly`);
        return;
    }
    });

    sleep(1);
}

// setup configuration
export function setup() 
{
    logInDebugMode(`==========================SETUP BEGINS==========================`)
    // log the date & time start of the test
    logInDebugMode(`Start of test: ${formatDate(new Date())}`)

    // log the test type
    logInDebugMode(`Test executed: ${Execution}`)

    // register a new user and authenticate via a Bearer token.
    let response = http.post(`${BASE_URL}/user/register/`, 
    {
        first_name: 'Crocodile',
        last_name: 'Owner', 
        username: USERNAME,
        password: PASSWORD,
    }); 
  
    const isSuccessfulRequest = check(response, 
    { 
        'created user': (r) => r.status === 201 //Created
    }); 
  
    if (isSuccessfulRequest)
    {
        logInDebugMode(`The user ${USERNAME} was created successfully!`);
    }
    else 
    {
        logInDebugMode(`There was a problem creating the user ${USERNAME}. It might be existing, so please modify it on the executor bat file`);
        logInDebugMode(`The http status is ${response.status}`);        
        logInDebugMode(`The http error is ${response.error}`);        
    }

    let loginResponse = http.post(`${BASE_URL}/auth/token/login/`, 
    {
        username: USERNAME,
        password: PASSWORD
    });
     
    let authorizationToken = loginResponse.json('access');
    let logInSuccessful = check(authorizationToken, 
    { 
        'logged in successfully': () => authorizationToken !== '', 
    });

    if (logInSuccessful)
    {
        logInDebugMode(`Logged in successfully with the token: ${authorizationToken}`); 
    }

    logInDebugMode(`==========================SETUP ENDS==========================`)

    return authorizationToken; // this will be passed as parameter to all the exported functions
}