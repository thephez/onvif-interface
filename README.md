This basic ONVIF test client provides an interface to connect to ONVIF compliant devices to retrieve device details, test PTZ control, and subscribe to device events.

# Features
 - Connect by IP/Port with or without authentication
 - Retrieve device info (Model, Firmware, Serial #, Hardware ID, and Time)
 - Retrieve supported services and their capabilities (analytics, events, extensions, imaging, media, PTZ, etc.)
 - Retrieve media information (URIs for camera streams)
 - Enable PTZ for supported devices
 - Subscribe to device events

Note: This application was tested with Bosch and Samsung cameras primarily.  One Axis camera was also used but it did not support all features.  Also, there are nuances in how various vendors support the ONVIF standard so there are likely aspects of the application that will not work with some devices without further development.
