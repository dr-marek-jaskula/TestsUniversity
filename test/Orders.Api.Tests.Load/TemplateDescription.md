## Load Testing

Load testing is about testing the performance, stability, and stress points of the application in the E2E manner.

In this case, we will define different scenarios that will send multiple requests per second and examine the response time and other metrics.

To run the test we run the application, open the terminal, navigate to the directory where the test file is present and then type:
```cmd
 k6 run <fileName>
```

## Execution Types

Execution type is the collection of scenarios that are use for the single purpose.

We distinguish:
- smoke
	- It is a regular load test, configured for minimal load. Run a smoke test to check every if the script is ok and system does not throw any errors.
- load
	- Load Testing is a type of Performance Testing used to determine a system's behavior under both normal conditions.
- stress
	- Stress Testing is a type of load testing used to determine the limits of the system. The purpose of this test is to verify the stability and reliability of the system under extreme conditions.
- soak
	- Soak testing is concerned with reliability over a longer period of time.
- spike
	- Test api when requests rapidly increases for a short period of time and then rapidly decreases to the previous request amount

Execution is a selected execution type.

## Scenario

Scenario is an guide how the requests should be send - in what amount and frequency.
K6 allows to use multiple, well build scenarios, for which we set the parameters.

The preferred one for needs of this scope is **ramping-arrival-rate** scenario type, called **executor**. This is due to its flexibility and great control on the test process.

**ramping-arrival-rate**
If you need your tests to not be affected by the system-under-test's performance, and would like to ramp the number of iterations up or down during specific periods of time.

For k6 we specify the following parameters of the **ramping-arrival-rate** executor:
- startTime 
	- Time offset since the start of the test, at which point this scenario should begin execution.
- startRate
	- Number of iterations to execute each second (timeUnit is "1s" by default)
- preAllocatedVUs
	- Number of VUs to pre-allocate before test start to preserve runtime resources. We should have at least some
- stages
	- Array of objects that specify the target number of iterations to ramp up or down to.

## Thresholds

These are the requirements that need to be satisfied by the test. We can have multiple thresholds.

We can define multiple thresholds for one action:
```js
'http_req_duration': ['p(95)<500', 'p(99)<1500'],
```

The p(95)<500 means that 95% of the requests have to have the response time lower than 0.5 second.

We can also use tagged threshold (it is a good practice)
```js
'http_req_duration{name:Read}': ['avg<500', 'max<1000'],
```

## Setup

Setup is a function that will be executed before the tests. It is commonly used to log some initial data like date and test type.

Also setup can be used to register and/or log the user and obtain the authentication token.