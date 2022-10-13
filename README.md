# Assignment in System Development
WPF with Azure Functions

 - Service.ClimateApp is an WPF-App that administrates devices from Azure IotHub. For the moment it gets all devices that is located in the kitchen. You can delete the device in the IotHub from here.
 - ConnectingDevice is a simulation of an IoT device written in WPF. If there are no connectionString in the storage solution it creates one by contacting the RESTFUL Web Api. It then gets it´s deviceTwin and if thats possible a message in the app says "Device Connected".
