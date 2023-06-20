# KLA - UniqueIdsScanner


 This project is a console-based application that deserialize KLA's company XML-Files then verify's or verify+update them with SqlServer-DB that holds the information for all of the versions.


### Features: 
 * User authentication.
  
 * Deserialize KLA's XML-Files and verify the data from the file with the database.
  
 * if the verify pass with no problems you can update the Dataabase with your new data.
  
 * Generate reports and visualize progress.

### Technologies Used:
* Frontend: CMD, SQLPAD.
* Backend: C#.
* Database: SQL Server.

### Installation
* Clone the repository: git clone https://github.com/Eli2694/KLA
* Navigate to the project directory: cd project
* restore the required dependencies: dotnet restore UniqueIdsScannerUI/UniqueIdsScannerUI.csproj
* Set up the required information in appconfig.json
* Build the app with: dotnet build UniqueIdsScannerUI/UniqueIdsScannerUI.csproj
* Navigate to the execution directory: cd path UniqueIdsScannerUI/UniqueIdsScannerUI/bin/debug/bin
* Start the application: UniqueScanner.exe 

### Usage not finished!!!:
* Register a new account or log in with your existing credentials.
* Create a new task by clicking the "Add Task" button.
* Fill in the task details, such as title, description, due date, and category.
* Save the task and it will be added to your task list.
* Update or delete tasks as needed.
* Use the search and filter options to find specific tasks.
* Collaborate with other users by sharing tasks with them.
* Generate reports to track your task progress over time.


