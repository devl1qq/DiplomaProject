This Git repository is dedicated to ERP Acumatica projects. It contains code for three primary modules:

Logistics Module – Includes integration with Google Maps to enhance route planning and delivery operations.

Banking Module – Features integration with the Opayo payment gateway for secure financial transactions.

Core Business Module – Introduces custom logic enhancements to standard Acumatica screens and includes entirely new custom screens. These additions support more efficient and tailored business processes for clients.

Below is a brief overview of the key logic implemented in this project:

Files with "DAC" in the namespace
These represent custom database tables or extensions of existing Acumatica tables. They include additional custom fields and serve as the foundation for storing new data structures.

Files with "Graph" in the namespace
These contain the business logic for Acumatica screens. They define custom screen behaviors such as event handlers (triggered by user actions), buttons, and SQL views used in grids, forms, popups, headers, and more.

Files with "Helper" or "Constants" in the namespace
These include utility classes and shared logic used across different modules. They also define constant variables for standardized warning/error messages and SQL statements.

(To see detailed info about code check commits)

  Local Deployment Instructions
  To run this project locally, follow these steps:
  
  Install Requirements
  
  Install Acumatica version 24.109.0016 (or any compatible 24R1 version).
  
  Set up Microsoft SQL Server (MSSQL).
  
  Create a New Instance
  
  Use the Acumatica Instance Wizard to create a new instance.
  
  Select the SalesDemo test data during setup.
  
  Access the Instance
  
  Open your new instance in a browser.
  
  Log in with the username admin and the password you set during installation.
  
  After logging in, set your desired new password if prompted.
  
  Import and Publish Customizations
  
  Navigate to the Customization Projects screen (Screen ID: SM204505).
  
  Import and publish the customization projects in the following order:
  
     1. - DefaultData
      
     2. - OpayoERPProject
      
     3. - MileageERPProject
      
     4. - MainERPProject
    
  Publishing in this sequence ensures all dependencies and features are correctly loaded.
