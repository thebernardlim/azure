# Bernard Farms

## Problem / Goal

Show correlation between water sprinkled against the amount of harvest per farm.
Project will strictly use Azure services and use follow the Lambda architecture.

## Solution

Services Used:
- Azure Databricks
- Azure Data Factory 
- Azure Synapse Analytics aka Data Warehouse
- Azure SQL Database
- Azure CosmosDB
- Azure Data Lake
- Azure Streaming Analytics
- Azure Event Hubs

### Initial Implementation

Lambda architecture has the following 3 layers:

**Hot Layer**
- **Sample App** - To depict farm sensors tracking the amount of water sprinkled 
- **Event Hubs** - Ingestion service to receive events from the sensors
- **Streaming Analytics** – Using Event Hubs as an input source, showing real-time analytics to a Power BI output.

**Cold Layer**
- **Data Lake** - Store the daily CSV files being uploaded by farmers which indicate the amount of harvest produced per farm. Also store the Event Hub streamed-in Avro files 
- **Data Factory** 
    - Source: Both the CSV and Avro files
    - Pipeline components:
        - Databricks Notebook: Transform the Avro file into a Parquet file
        - DataFlow activity: A few transformation activities to join both files and return the required columns only
    - Output query into an Azure Synapse
- **Azure Synapse** - Has a table storing the water sprinkled & amount of harvest per farm. Connected to Power BI to show correlation.

**Serving Layer**
- **Power BI** - For analytics



#### High Level Architecture diagram

![alt text][logo]

[logo]: https://github.com/thebernardlim/azure/blob/master/bernard-farms/images/bernard-farms-architecture-diagram-v1.png ""

#### Data Factory

##### Pipeline - Overall

![alt text][logo1]

[logo1]: https://github.com/thebernardlim/azure/blob/master/bernard-farms/images/data-factory-pipeline-v1.PNG ""

##### Pipeline Activity - Data Flow

![alt text][logo2]

[logo2]: https://github.com/thebernardlim/azure/blob/master/bernard-farms/images/data-factory-pipeline-dataflow-v1.PNG ""

#### Problems Faced
- Currently, Data Factory cannot automatically detect schema from Avro files, hence a Databricks Notebook is required to transform the Avro files into Parquet files.

### Future Enhancements
