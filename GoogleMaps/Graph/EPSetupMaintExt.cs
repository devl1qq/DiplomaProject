using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using MileageCustomisation.DAC;
using MileageCustomisation.Helpers;
using PX.Data;
using PX.Objects.EP;
using RestSharp;
using static PX.SM.LicenseInfo;

namespace MileageCustomisation.Graph
{
    // todo: needs refactoring
    public class EPSetupMaintExt : PXGraphExtension<EPSetupMaint>
    {
        #region Actions

        #region ValidateGoogleApiKey
        public PXAction<EPSetup> ValidateGoogleApiKey;
        [PXUIField(DisplayName = "Validate API Key")]
        [PXButton]
        public virtual IEnumerable validateGoogleApiKey(PXAdapter adapter)
        {
            var key = Base.Setup.Current?.GetExtension<EPSetupExt>()?.UsrGooleMapsAPIKey;

            if (string.IsNullOrEmpty(key))
                throw new PXException("The key is empty.");

            if (!ValidateKey(key))
            {
                string message = 
@"The key Is not Valid.

To use Google Maps services, you need to enable the required APIs in Google Cloud Console. Please follow these steps:

1. Go to Google Cloud Console: https://console.cloud.google.com/
2. Select your project (or create a new one if needed).
3. Navigate to 'APIs & Services' > 'Enabled APIs & Services'.
4. Click 'Enable APIs & Services' at the top.
5. Search for and enable the following APIs:
   - Directions API
   - Maps JavaScript API
   - Geocoding API
   - Routes API
6. Ensure your API key has no restrictions blocking these services:
   - Go to 'APIs & Services' > 'Credentials'
   - Click on your API key
   - Under 'API Restrictions', ensure the required APIs are selected or set to 'Unrestricted' if needed.
7. Make sure billing is enabled, as some APIs require an active billing account.

After enabling these services, retry your request. If you still encounter issues, double-check your API key, restrictions, and billing settings.";

                throw new PXException(message);
            }

            throw new PXException("The key is valid.");
        }
        #endregion

        #endregion

        #region Methods

        private bool ValidateKey(string key)
        {
            var client = new RestClient();
            foreach (var api in GetGoogleMapsValidationEndpoints(key))
            {
                var endpoint = api.Value;
                var apiService = api.Key;

                try
                {
                    RestRequest request = new RestRequest(endpoint);

                    // only for Routes API (post request with body, headers)
                    if (apiService == "Routes API")
                    {
                        var requestBody = new
                        {
                            origin = new { location = new { latLng = new { latitude = 40.7128, longitude = -74.0060 } } }, // New York
                            destination = new { location = new { latLng = new { latitude = 34.0522, longitude = -118.2437 } } }, // Los Angeles
                            travelMode = "DRIVE"
                        };
                        request.Method = Method.Post;
                        request.AddHeader("X-Goog-Api-Key", key);
                        request.AddHeader("X-Goog-FieldMask", "routes.distanceMeters");
                        request.AddJsonBody(requestBody);
                    }

                    // get response
                    var response = client.ExecuteAsync(request).Result;
                    if (!response.IsSuccessful || response.Content == null || response.Content.Contains("error_message"))
                        return false;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return true;
        }
        private Dictionary<string, string> GetGoogleMapsValidationEndpoints(string key)
        {
            return new Dictionary<string, string>
            {
                {
                    "Directions API",
                    $"https://maps.googleapis.com/maps/api/directions/json?origin=New+York,NY&destination=Los+Angeles,CA&key={key}"
                },
                {
                    "Maps JavaScript API",
                    $"https://maps.googleapis.com/maps/api/js?key={key}"
                },
                {
                    "Geocoding API",
                    $"https://maps.googleapis.com/maps/api/geocode/json?address=New+York&key={key}"
                },
                {
                    "Routes API",
                    $"https://routes.googleapis.com/directions/v2:computeRoutes"
                }
            };
        }

        #endregion

    }
}
