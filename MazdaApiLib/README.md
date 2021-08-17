# MazdaApiLib
This is an API client for the MyMazda (Mazda Connected Services) API. This is the API used by the MyMazda mobile app ([iOS](https://apps.apple.com/us/app/mymazda/id451886367)/[Android](https://play.google.com/store/apps/details?id=com.interrait.mymazda)).

Note: There is no official API, and this library may stop working at any time without warning.

This library is a port of the **pymazda** library by **bdr99**.

# Installation

To install the latest release from [PyPI](https://pypi.org/project/pymazda/), run `pip3 install pymazda`.

# Quick Start

This example initializes the API client and gets a list of vehicles linked to the account. Then, for each vehicle, it gets the vehicle status.

```C#
using System.Net.Http;
using WingandPrayer.MazdaApi;
using WingandPrayer.MazdaApi.Model;

namespace apitest
{
    class Program
    {
        static void Main(string[] args)
        {
            using HttpClient httpClient = new();

            // initialise the MazdaAPI client
            MazdaApiClient mazdaApiClient = new("my email", "my password", "my region", httpClient);

            // loop through the registered vehicles
            foreach (VehicleModel vehicleModel in mazdaApiClient.GetVehicles())
            {
                // check that the vehicle is fully registered correctly.
                if (vehicleModel.VinRegistStatus == 3)
                {
                    // get the vehicle status from the api
                    VehicleStatus vehicleStatus = mazdaApiClient.GetVehicleStatus(vehicleModel.Id);

                    // see if the vehicle is an electric vehicle
                    if (vehicleModel.IsEvVehicle)
                    {
                        EvVehicleStatus evVehicleStatus  = mazdaApiClient.GetEvVehicleStatus(vehicleModel.Id);
                    }
                }
            }
        }
    }
}
```

You will need the email address and password that you use to sign into the MyMazda mobile app. Before using this library, you will need to link your vehicle to your MyMazda account using the app. You will also need the region code for your region. See below for a list of region codes.

When calling these methods, it may take some time for the vehicle to respond accordingly. This is dependent on the quality of the car's connection to the mobile network. It is best to avoid making too many API calls in a short time period of time, as this may result in rate limiting.

The sample code above shows using synchronous methods of the client but if required async methods are available.

# API Documentation

### MazdaApiClient(emailAddress,password,region,httpClient,useCachedVehicleList,logger) `constructor`

##### Summary

Constructs an instance of the MazdaApiClient that is used to access all the public methods.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| emailAddress | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The email address you use to log into the MyMazda mobile app |
| password | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The password you use to log into the MyMazda mobile app |
| region | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The code for the region in which your account was registered (MNAO - North America, MME - Europe, MJO - Japan) |
| httpClient | [System.Net.Http.HttpClient](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Net.Http.HttpClient 'System.Net.Http.HttpClient') | HttpClient used to communicate with MyMazda |
| useCachedVehicleList | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | A flag that when set to true caches calls to methods that return vehicles. (Optional, defaults to false) |
| logger | [Microsoft.Extensions.Logging.ILogger{WingandPrayer.MazdaApi.MazdaApiClient}](#T-Microsoft-Extensions-Logging-ILogger{WingandPrayer-MazdaApi-MazdaApiClient} 'Microsoft.Extensions.Logging.ILogger{WingandPrayer.MazdaApi.MazdaApiClient}') | An ILogger that can be used for debugging and tracing purposes. (Optional, defaults to null) |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetAssumedLockState-System-String-'></a>
### GetAssumedLockState(internalVin) `method`

##### Summary

Get the assumed lock state of a vehicle.

##### Returns

A bool to indicate the assumed lock state or null if the lock state cannot be determined

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

##### Remarks

This method derives the lock state from both the actual lock state that is read from the vehicle and the attempts by this to lock and unlock the vehicle by this api
It is required because there is often a delay between locking the vehical and the status changing with the api

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetAvailableService-System-String-'></a>
### GetAvailableService(internalVin) `method`

##### Summary

Get the available services for a given vehicle.

##### Returns

An AvailableService object containing the available services

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetAvailableServiceAsync-System-String-'></a>
### GetAvailableServiceAsync(internalVin) `method`

##### Summary

Get the available services for a given vehicle asynchronously.

##### Returns

An AvailableService object containing the available services

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetEvVehicleStatus-System-String-'></a>
### GetEvVehicleStatus(internalVin) `method`

##### Summary

Get the vehicle status information relating to an electric vehicle from the API

##### Returns

An EvVehicleStatus

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetEvVehicleStatusAsync-System-String-'></a>
### GetEvVehicleStatusAsync(internalVin) `method`

##### Summary

Get the vehicle status information relating to an electric vehicle from the API asynchronously.

##### Returns

An EvVehicleStatus

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetRawVehicleStatus-System-String-'></a>
### GetRawVehicleStatus(internalVin) `method`

##### Summary

Get the raw vehicle status information from the API

##### Returns

A MazdaApiRawVehicleStatus

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

##### Remarks

This information has fields that are not applicable to electric vehicles please see GetEvVehicleStatus

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetRawVehicleStatusAsync-System-String-'></a>
### GetRawVehicleStatusAsync(internalVin) `method`

##### Summary

Get the raw vehicle status information from the API asynchrounously.

##### Returns

A MazdaApiRawVehicleStatus

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

##### Remarks

This information has fields that are not applicable to electric vehicles please see GetEvVehicleStatusAsync

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetRawVehicles'></a>
### GetRawVehicles() `method`

##### Summary

Get the raw vehicle(s) information from the API

##### Returns

A MazdaApiVehicles describing the raw information for all api vehicles

##### Parameters

This method has no parameters.

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetRawVehiclesAsync'></a>
### GetRawVehiclesAsync() `method`

##### Summary

Get the raw vehicle(s) information from the API asynchronously.

##### Returns

A MazdaApiVehicles describing the raw information for all api vehicles

##### Parameters

This method has no parameters.

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetRemotePermissions-System-String-'></a>
### GetRemotePermissions(vin) `method`

##### Summary

Get the remote permissions for a given vehicle.

##### Returns

A RemoteControlPermissions object containing the remote permissions

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| vin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The vehicle identity number fo the vehicle |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetRemotePermissionsAsync-System-String-'></a>
### GetRemotePermissionsAsync(vin) `method`

##### Summary

Get the remote permissions for a given vehicle asynchronously.

##### Returns

A RemoteControlPermissions object containing the remote permissions

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| vin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The vehicle identity number fo the vehicle |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetVehicleStatus-System-String-'></a>
### GetVehicleStatus(internalVin) `method`

##### Summary

Get the simplified vehicle status information from the API

##### Returns

A VehicleStatus

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetVehicleStatusAsync-System-String-'></a>
### GetVehicleStatusAsync(internalVin) `method`

##### Summary

Get the simplified vehicle status information from the API asynchronously.

##### Returns

A VehicleStatus

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetVehicles'></a>
### GetVehicles() `method`

##### Summary

Get the list of vehicles from the API

##### Returns

A list of VehicleModel describing the raw information for all api vehicles

##### Parameters

This method has no parameters.

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-GetVehiclesAsync'></a>
### GetVehiclesAsync() `method`

##### Summary

Get the list of vehicles from the API asynchronously.

##### Returns

A list of VehicleModel describing the raw information for all api vehicles

##### Parameters

This method has no parameters.

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-LockDoor-System-String-'></a>
### LockDoor(internalVin) `method`

##### Summary

Locks the doors of the vehicle.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-LockDoorAsync-System-String-'></a>
### LockDoorAsync(internalVin) `method`

##### Summary

Locks the doors of the vehicle asynchronously.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-TurnOffHazzardLights-System-String-'></a>
### TurnOffHazzardLights(internalVin) `method`

##### Summary

Turns off the vehicle hazard lights.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-TurnOffHazzardLightsAsync-System-String-'></a>
### TurnOffHazzardLightsAsync(internalVin) `method`

##### Summary

Turns off the vehicle hazard lights asynchronously.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-TurnOnHazzardLights-System-String-'></a>
### TurnOnHazzardLights(internalVin) `method`

##### Summary

Turns on the vehicle hazard lights.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-TurnOnHazzardLightsAsync-System-String-'></a>
### TurnOnHazzardLightsAsync(internalVin) `method`

##### Summary

Turns on the vehicle hazard lights asynchronously.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-UnlockDoor-System-String-'></a>
### UnlockDoor(internalVin) `method`

##### Summary

Unlocks the doors of the vehicle.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |

<a name='M-WingandPrayer-MazdaApi-MazdaApiClient-UnlockDoorAsync-System-String-'></a>
### UnlockDoorAsync(internalVin) `method`

##### Summary

Unlocks the doors of the vehicle asynchronously.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| internalVin | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The internal vehicle identity number for the vehicle which can be found with calls to methods that return vehicles |
