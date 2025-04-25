using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PX.Data;

namespace MileageCustomisation.Helpers
{
    public abstract class GoogleMapsApiHelper
    {
        public static double CalculateDistance(string originAddress, string destinationAddress, string apiKey)
        {
            var originCoords = Geocoding.GetCoordinatesFromAddress(originAddress, apiKey);
            var destinationCoords = Geocoding.GetCoordinatesFromAddress(destinationAddress, apiKey);
            if (originCoords == null || destinationCoords == null)
                throw new PXException("Failed to get coordinates for addresses.");

            var distance = Routes.GetDistanceFromCoords(originCoords, destinationCoords, apiKey);
            if (distance == null)
                throw new PXException("Failed to get distance from coordinates.");

            return distance.GetValueOrDefault();
        }

        private abstract class Geocoding
        {
            public static (double latitude, double longitude)? GetCoordinatesFromAddress(string address, string apiKey)
            {
                string formattedAddress = Uri.EscapeDataString(address);
                string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={formattedAddress}&key={apiKey}";

                var client = new RestClient();
                var request = new RestRequest(url);

                var response = client.ExecuteAsync(request).Result;
                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var data = JsonDocument.Parse(response.Content);

                    if (data.RootElement.TryGetProperty("error_message", out var errorMessage))
                        throw new PXException(errorMessage.GetRawText());

                    if (data.RootElement.TryGetProperty("results", out var results) && results.GetArrayLength() > 0)
                    {
                        var location = results[0].GetProperty("geometry").GetProperty("location");
                        double lat = location.GetProperty("lat").GetDouble();
                        double lng = location.GetProperty("lng").GetDouble();
                        return (lat, lng);
                    }
                }
                else
                    throw new PXException(response.Content);

                return null;
            }
        }
        private abstract class Routes
        {
            public static double? GetDistanceFromCoords(
                (double latitude, double longitude)? originCoords,
                (double latitude, double longitude)? destinationCoords,
                string apiKey
            )
            {
                string url = "https://routes.googleapis.com/directions/v2:computeRoutes";
                var client = new RestClient();
                var request = new RestRequest(url);

                request.AddHeader("X-Goog-Api-Key", apiKey);
                request.AddHeader("X-Goog-FieldMask", "routes.distanceMeters");

                var requestBody = new
                {
                    origin = new
                    {
                        location = new
                        {
                            latLng = new
                            {
                                latitude = originCoords.GetValueOrDefault().latitude, 
                                longitude = originCoords.GetValueOrDefault().longitude
                            }
                        }
                    },
                    destination = new
                    {
                        location = new
                        {
                            latLng = new
                            {
                                latitude = destinationCoords.GetValueOrDefault().latitude,
                                longitude = destinationCoords.GetValueOrDefault().longitude
                            }
                        }
                    },
                    travelMode = "DRIVE"
                };
                request.AddJsonBody(requestBody);

                var response = client.ExecuteAsync(request, Method.Post).Result;
                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var data = JsonDocument.Parse(response.Content);

                    if (data.RootElement.TryGetProperty("error_message", out var errorMessage))
                        throw new PXException(errorMessage.GetRawText());

                    double distanceMeters = data.RootElement
                        .GetProperty("routes")[0]
                        .GetProperty("distanceMeters")
                        .GetDouble();

                    return distanceMeters * 0.000621371; // convert to miles
                }

                throw new PXException(response.Content);
            }
        }

    }
}
