## Azure Data Factory Utility Function App

### ADF-Utility
Solution to store various functions to communicate with ADF REST API.

#### Credits
Source code origin credits to: https://github.com/mrpaulandrew/BlogSupportingContent/blob/master/Get%20Any%20Azure%20Data%20Factory%20Pipeline%20Run%20Status%20with%20Azure%20Functions/PipelineStatusChecker/PipelineStatusChecker/Functions.cs

Made my own modifications on top of original source code.

#### Functions

- GetAndWaitForStatusByName: Get aafnd wait for status by pipeline name
- GetStatusByNameAndRunId: Get and wait for status by pipeline name and run ID
- GetStatusByNameOnly.cs: Get and wait for status of latest run of pipeline
- RunSingleInstancePipelineByName.cs: Run pipeline by name, and only run when there is no existing run in progress.

### adf-utility-postman-script
Postman script to interact with Function App.

To use, call 'Get AAD Token' first to get access token for subsequent API calls.
Ensure that there is already an AAD App registered to represent the client. Set settings as necessary within the global variables