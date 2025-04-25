using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MileageCustomisation.DAC;
using PX.Data;
using PX.Objects.EP;
using PX.SM;
using PX.Web.UI;
using static PX.CS.RMStyle;

namespace MileageCustomisation.Graph
{

    public class ExpenseClaimDetailEntryExt : PXGraphExtension<ExpenseClaimDetailEntry>
    {
        public class ExpenseClaimDetailEntry_ExpenseClaimDeatilsMileageExt : ExpenseClaimDeatilsMileageExt<ExpenseClaimDetailEntry.ExpenseClaimDetailEntryExt, ExpenseClaimDetailEntry> { }

        #region Google API Integration

        #region Views

        // dummy DAC without any logic (only to show popup)
        public class GoogleMapsDummy : DummyTable { }

        public PXSelect<GoogleMapsDummy> GoogleMapsView;

        #endregion

        #region Actions

        #region ViewGoogleMapsRoute
        public PXAction<EPExpenseClaimDetails> ViewGoogleMapsRoute;
        [PXUIField(DisplayName = "View Route")]
        [PXButton]
        public virtual void viewGoogleMapsRoute()
        {
            GoogleMapsView.AskExt();
        }
        #endregion

        #endregion

        #region Initialization

        // append JavaScript to page dynamically
        public override void Initialize()
        {
            base.Initialize();

            var page = HttpContext.Current?.Handler as PXPage;
            if (page == null)
                return;

            page.Load += AppendCssStyles;
            page.Load += AppendGoogleMapsNodes;
            page.Load += AppendInitMapFunction;
            page.Load += AppendSetRouteFunction;
            page.Load += LoadGoogleMapsAPI;
        }

        private void AppendCssStyles(object sender, EventArgs e)
        {
            // This script creates CSS styles and adds them to the page's body.

            var page = (Page)sender;
            var script = @"
                let style = document.createElement(""style"");
                style.type = ""text/css"";

                style.innerHTML = `
                    #container {
                        display: flex;
                        align-items: center;
                        flex-direction: column;
                        width: 100%;
                        height: 100%;
                        box-sizing: border-box;
                    }

                    #map-container {
                        height: 100%;
                        width: 100%;
                        display: flex;
                        box-sizing: border-box;
                        overflow: hidden;
                    }

                    #map {
                        flex-grow: 4;
                        height: 100%;
                    }

                    #panel {
                        overflow: auto;
                        height: 100%;
                        width: 350px;
                        font-size: 0.9rem;
                    }

                    .panel-invisible {
                        display: none;
                    }
                `;

                document.head.appendChild(style);
            ";

            page.ClientScript.RegisterStartupScript(GetType(), "AppendStyles", script, true);
        }
        private void AppendGoogleMapsNodes(object sender, EventArgs e)
        {
            // This script generates the following HTML elements and appends them to the popup.
            // The div#map element is an essential part of the Google Maps API and is required for rendering maps
            // The div#panel element is also part of the Google Maps API, but it is optional and can be removed if not needed.
            // To remove div#panel element, you also need to delete the line 'panel: document.getElementById("panel")' from the initMap() function.

            // <div id = "container" >
            //      <div id="map-container">
            //          <div id="map"></ div >
            //          <div id="panel" class="panel-invisible"></div>
            //      </div>
            // </div>

            var page = (Page)sender;
            var script = @"
                // create container
                let containerElement = document.createElement(""div"");
                containerElement.id = ""container"";
                
                // create map-container
                let mapContainerElement = document.createElement(""div"");
                mapContainerElement.id = ""map-container"";
                
                // create map
                let mapElement = document.createElement(""div"");
                mapElement.id = ""map"";
                
                // create panel
                let panelElement = document.createElement(""div"");
                panelElement.id = ""panel"";
                panelElement.classList.add(""panel-invisible"");
                
                // append all children
                mapContainerElement.appendChild(mapElement);
                mapContainerElement.appendChild(panelElement);
                containerElement.appendChild(mapContainerElement);
                
                // append nodes to the popup panel
                document.getElementById(""ctl00_phG_CstSmartPanelGoogleMapsView_cont"").firstChild.appendChild(containerElement);
            ";

            page.ClientScript.RegisterStartupScript(GetType(), "appendGoogleMapsNodes", script, true);
        }
        private void AppendInitMapFunction(object sender, EventArgs e)
        {
            // This script defines the initMap() function and initializes variables.
            // The initMap() function is a required part of the Google Maps API for rendering the map.
            // The variables are defined in the global context because they are used in the setRoute() function.

            var page = (Page)sender;
            var script = @"

                let map;
                let directionsService;
                let directionsRenderer;                

                function initMap() {
                    // Initialize the map
                    map = new google.maps.Map(document.getElementById(""map""), {
                        center: { lat: 0.0, lng: 0.0 },
                        zoom: 2,
                    });

                    // Create a DirectionsService object to calculate routes
                    directionsService = new google.maps.DirectionsService();

                    // Create a DirectionsRenderer object to display the route on the map
                    //directionsRenderer = new google.maps.DirectionsRenderer({
                    //    panel: document.getElementById(""panel""),
                    //});
                    //directionsRenderer.setMap(map);
                }
            ";

            page.ClientScript.RegisterStartupScript(GetType(), "appendInitMapFunction", script, true);
        }
        private void AppendSetRouteFunction(object sender, EventArgs e)
        {
            // This script defines the SetRoute() function and triggers it when the user presses the 'VIEW ROUTE' button (aspx).
            // The function is used to change routes on Google Maps.

            var page = (Page)sender;
            var script = @"

                var originLocation;
                var destLocation;

                function ResetMap(){
                     // clear routes from map
                     if (directionsRenderer){
                         directionsRenderer.setDirections({ routes: [] });
                     }
                    
                     // reset map
                     if (map){
                         map.setCenter({ lat: 0.0, lng: 0.0 });
                         map.setZoom(2);
                     }

                     // Hide Panel
                     document.getElementById(""panel"").classList.add(""panel-invisible"");
                }
                
                function SetRoute() {
                    let startLocation = px_alls['CstUsrFromLocation']?.getValue() ?? '';
                    let endLocation = px_alls['CstUsrToLocation']?.getValue() ?? '';

                    if (!directionsRenderer){
                        // Create a DirectionsRenderer object to display the route on the map
                        directionsRenderer = new google.maps.DirectionsRenderer({
                            panel: document.getElementById(""panel""),
                        });
                        directionsRenderer.setMap(map);
                    }
                    
                    if (originLocation === startLocation && destLocation === endLocation)
                        return;

                    if (!startLocation || !endLocation){
                        ResetMap();
                        return;
                    }

                    originLocation = startLocation;
                    destLocation = endLocation;

                    directionsService.route({
                        origin: startLocation,
                        destination: endLocation,
                        travelMode: google.maps.TravelMode.DRIVING, // Other options: WALKING, BICYCLING, TRANSIT
                        unitSystem: google.maps.UnitSystem.IMPERIAL // // Use miles (imperial units)
                    },
                        (result, status) => {
                            if (status === google.maps.DirectionsStatus.OK) {
                                console.log('Route requested.');
                                directionsRenderer.setDirections(result);

                                //Automatically adjust the zoom and bounds to fit the route
                                const bounds = new google.maps.LatLngBounds();
                                const route = result.routes[0];
                                route.legs.forEach((leg) => {
                                    bounds.extend(leg.start_location);
                                    bounds.extend(leg.end_location);
                                });
                                map.fitBounds(bounds); // Adjusts the map view to fit the route

                                document.getElementById(""panel"").classList.remove(""panel-invisible"");
                            } else {
                                console.error(""Error fetching directions:"", status);
    
                                // hide panel
                                //document.getElementById(""panel"").classList.add(""panel-invisible"");
                                
                                ResetMap();
                            }
                        }
                    );
                }
            ";

            page.ClientScript.RegisterStartupScript(GetType(), "setRoute", script, true);
        }
        private void LoadGoogleMapsAPI(object sender, EventArgs e)
        {
            // This script loads the Google Maps API library and adds it to the page.
            // Key is required for this API.

            var page = (Page)sender;

            var key = this.GetGoogleMapsAPIKey();
            var url = $"https://maps.googleapis.com/maps/api/js?key={key}&callback=initMap&libraries=places&language=en&loading=async";
            var script = @"
                try {
                    const script = document.createElement('script');
                    script.src = `[url]`;
                    script.defer = true;
                    script.async = true;
                    script.crossOrigin = 'anonymous'; 
                    script.onload = () => console.error(`Google Maps API loaded.`);
                    script.onerror = () => console.error(`Error loading script: [url]`);
                    document.body.appendChild(script);
                } catch(error) {}
            ".Replace("[url]", url);

            page.ClientScript.RegisterStartupScript(GetType(), "loadGoogleMapsAPI", script, true);
        }
        private string GetGoogleMapsAPIKey()
        {
            var key = Base.epsetup.Current?.GetExtension<EPSetupExt>()?.UsrGooleMapsAPIKey;
            return string.IsNullOrEmpty(key) ? "null" : key;
        }

        #endregion

        #endregion

        #region Events
        protected virtual void _(Events.RowSelected<EPExpenseClaimDetails> e)
        {
            var row = e.Row;
            if (row == null)
                return;

            ViewGoogleMapsRoute.SetEnabled(IsMileageItem(row));
            ViewGoogleMapsRoute.SetVisible(IsMileageItem(row));
        }

        #endregion

        #region Methods

        private bool IsMileageItem(EPExpenseClaimDetails row)
        {
            var epSetup = Base.epsetup.Current;
            var epSetupExt = epSetup.GetExtension<EPSetupExt>();

            return row != null && row.InventoryID == epSetupExt.UsrMileageItem;
        }

        #endregion

        //public override void Initialize()
        //{
        //    PXGraph.InstanceCreated.AddHandler<UploadFileMaintenance>((graph) =>
        //        graph.RowPersisted.AddHandler<UploadFile>((sender, e) =>
        //        {
        //            var file = (UploadFile)e.Row;
        //            if (e.TranStatus != PXTranStatus.Completed || file == null)
        //                return;

        //            var detail = Base.ClaimDetails.Current;
        //            if (detail == null)
        //                return;

        //            var expenseClaimDetailEntry = PXGraph.CreateInstance<ExpenseClaimDetailEntry>();
        //            expenseClaimDetailEntry.ClaimDetails.Update(detail);

        //            var anyFileAttached = PXNoteAttribute.GetFileNotes(expenseClaimDetailEntry.ClaimDetails.Cache, detail).Any();
        //            expenseClaimDetailEntry.ClaimDetails.Cache.SetValueExt<EPExpenseClaimDetailsExt.usrReceiptAttached>(detail, anyFileAttached);
        //            expenseClaimDetailEntry.ClaimDetails.Cache.SetValueExt<EPExpenseClaimDetailsExt.usrToLocation>(detail, "tettstststststst");

        //            expenseClaimDetailEntry.Save.Press();
        //            Base.Cancel.Press();
        //        }));

        //    base.Initialize();
        //}
    }
}
