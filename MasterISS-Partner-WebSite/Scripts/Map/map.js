function MapJS(containerId, latitudeInput, longitudeInput) {
    var selectButton = $(".customer-localition-info-button");
    function getAddress(latLng) {
        geocoder.geocode({ 'latLng': latLng },
            function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    if (results[0]) {
                        $('#selected-place-label').html(results[0].formatted_address);
                    }
                    else {
                        $('#selected-place-label').html("Not Found !");
                    }
                }
                else {
                    $('#selected-place-label').html(latLng.lat() + "," + latLng.lng());
                }
            }
        );
    }
    var geocoder;
    selectButton.click(
        function a () {
            //container.toggle("fast");
            var map = new google.maps.Map(document.getElementById('map_locations'), {
                center: { lat: 38.70962332680603, lng: 35.484885897812916 },
                zoom: 6,
                mapTypeId: 'roadmap',
            });

            geocoder = new google.maps.Geocoder();
            google.maps.event.addListener(map, 'click', function (event) {
                $(latitudeInput).val(event.latLng.lat());
                $(longitudeInput).val(event.latLng.lng());
                getAddress(event.latLng);
            });

            var searched_name = document.getElementById('selected-place-label');
            var input = document.getElementById('searced-place-input');

            var searchBox = new google.maps.places.SearchBox(input);

            map.controls[google.maps.ControlPosition.TOP_LEFT].push(searched_name);//adds the input to the map
            map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);


            map.addListener('bounds_changed', function () {
                searchBox.setBounds(map.getBounds());
            });


            var markers = [];
            searchBox.addListener('places_changed', function () {
                var places = searchBox.getPlaces();

                if (places.length == 0) {
                    return;
                }

                markers.forEach(function (marker) {
                    marker.setMap(null);
                });
                markers = [];
                var bounds = new google.maps.LatLngBounds();
                places.forEach(function (place) {
                    if (!place.geometry) {
                        return;
                    }
                    var icon = {
                        url: place.icon,
                        size: new google.maps.Size(71, 71),
                        origin: new google.maps.Point(0, 0),
                        anchor: new google.maps.Point(17, 34),
                        scaledSize: new google.maps.Size(25, 25)
                    };


                    markers.push(new google.maps.Marker({
                        map: map,
                        icon: icon,
                        title: place.name,
                        position: place.geometry.location
                    }));

                    if (place.geometry.viewport) {

                        bounds.union(place.geometry.viewport);
                    } else {
                        bounds.extend(place.geometry.location);
                    }
                });
                map.fitBounds(bounds);
            });
        }
    );

    //closeButton.click(function () {
    //    container.hide();
    //})

    var map = document.createElement('script');
    map.type = 'text/javascript';
    map.src = 'https://maps.googleapis.com/maps/api/js?key=AIzaSyC2HjBa_I-GX4LOvp71kjtPoZQ4Uz-VBjo&libraries=places&callback=initAutocomplete';
    document.body.appendChild(map);
}