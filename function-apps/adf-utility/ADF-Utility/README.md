Azure Data Factory Utility Function

Comprises of various functions to communicate with ADF REST API.

Source code origin credits to: https://github.com/mrpaulandrew/BlogSupportingContent/blob/master/Get%20Any%20Azure%20Data%20Factory%20Pipeline%20Run%20Status%20with%20Azure%20Functions/PipelineStatusChecker/PipelineStatusChecker/Functions.cs

Functions:
- GetAndWaitForStatusByName: Get and wait for status by pipeline name
- GetStatusByNameAndRunId: Get and wait for status by pipeline name and run ID
- GetStatusByNameOnly.cs: Get and wait for status of latest run of pipeline
- RunPipelineByName.cs: Run pipeline by name, and only run when there is no existing run in progress.

Made my own modifications on top.
