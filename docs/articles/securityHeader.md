# HTTP Response Security Header
DNVGL.Web.Security provides extension methods to setup http response headers for ASP.NET Core application.

---
# Package Install

Ensure you have configured to package NuGet Package Source or find the instructions [here](./PackageInstall.md).

Package Manager Console
```
PM> `Install-Package DNVGL.OAuth.Web`
```

## 1. Basic Example

```cs
    public class Startup
    {
        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseDefaultHeaders();
            //...
        }
    }
```

* The package set up below default http response headers.

| Key | Value 
|---|---
|X-Xss-Protection|  1
|X-Frame-Options|SAMEORIGIN
|X-Content-Type-Options| no-referrer
|X-Permitted-Cross-Domain-Policies|none
|Expect-CT|enforce, max-age=7776000
|Strict-Transport-Security|max-age=15552000; includeSubDomains  
>If you have setup your own response headers before using the pacakge to setup default headers. You own reponse headers will be kept.  


* The package set up below default csp rule in http response headers.  

| Key | Value 
|---|---
|default-src|'self'
|object-src|'self'
|connect-src|'self' https://dc.services.visualstudio.com
|script-src|'self' https://www.recaptcha.net https://www.gstatic.com https://www.gstatic.cn
|font-src|'self' data: https://onedesign.azureedge.net https://veracitycdn.azureedge.net
|media-src|'self'
|worker-src|'self' blob:
|img-src|'self' https://onedesign.azureedge.net https://dnvglcom.azureedge.net https://cdnveracity.azureedge.net
|frame-src|'self' https://www.google.com https://www.recaptcha.net/
|style-src|'self' https://onedesign.azureedge.net

>If you have setup your own CSP before using the pacakge to setup default headers. You own CSP will be kept.  

## 2. Customize Response Header
The pacakge supports to overwrite the above default setting. This is a code sample to overwrite X-Frame-Options:

```cs
    public class Startup
    {
        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseDefaultHeaders(h=> h.Add("X-Frame-Options", "DENNY"));
            //...
        }
    }
```

## 3. Customize CSP in Response Header
The pacakge supports to overwrite the above default setting. This is a code sample to overwrite styleSrc:
 ```cs
     public class Startup
    {
        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseDefaultHeaders(h=> h.ReplaceDefaultContentSecurityPolicy(styleSrc: "'self' 'nonce-123456789909876543ghjklkjvcvbnm'"););
            //...
        }
    }
 ```

 Or extend the above default setting. This is a code sample to extend styleSrc:
 ```cs
     public class Startup
    {
        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseDefaultHeaders(h=> h.ExtendDefaultContentSecurityPolicy(styleSrc: "'nonce-123456789909876543ghjklkjvcvbnm'"););
            //...
        }
    }
 ```


 ## 4. Skip CSP in Response Header for specific requests.
By default, The package doesn't add csp into respsone for all http requests which url contain '/swagger/'.
It supports to overwrite the default skip logic. This is a code sample to skip all request which url contains '/nocsprequired/'.

```cs
    public class Startup
    {
        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseDefaultHeaders(h=> h.SkipContentSecurityPolicyForRequests((req) => req.Path.ToString().ToLowerInvariant().Contains("/nocsprequired/")));
            //...
        }
    }
```



## 5. Permissions Policy

Permissions Policy HTTP Header can be provided by your web server / web application in order to improve the security of your visitors and the data they may be accessing on your site.

**Standardized Features**

| Feature Name                    | Description                                                  |
| ------------------------------- | ------------------------------------------------------------ |
| accelerometer                   | The Accelerometer interface of the Sensor APIs provides on each reading the acceleration applied to the device along all three axes. |
| ambient-light-sensor            | The AmbientLightSensor interface of the Sensor APIs returns the current light level or illuminance of the ambient light around the hosting device. |
| autoplay                        | Controls the ability to have Media (Audio or Video) elements begin playback without user interaction in the current document. When this policy is disabled and there were no user gestures, the Promise returned by HTMLMediaElement.play() will reject with a DOMException. The autoplay attribute on `<audio>` and `<video>` elements will be ignored. |
| battery                         | The Battery Status API can be used to defer or scale back work when the device is not charging in or is low on battery. |
| camera                          | Manages access to Camera interfaces (physical and virtual).  |
| cross-origin-isolated           | Cross-origin isolation enables a web page to use powerful features such as SharedArrayBuffer, performance.measureUserAgentSpecificMemory(), high resolution timer with better precision, or the JS Self-Profiling API. This also impacts the "document-domain" permission when set (see below). |
| display-capture                 | A document's permissions policy determines whether any content in that document is allowed to use getDisplayMedia. |
| document-domain                 | Provides access to the deprecated "document.domain[=domain]" setter. When the "document-domain" feature is disabled, the setter will throw a "SecurityError" exception. In cases where crossOriginIsolated or originAgentCluster return true, the setter will do nothing. It is recommended to avoid using the document.domain setter, instead, use postMessage() or MessageChannel objects to communicate across origins in a safe manner. |
| encrypted-media                 | Encrypted Media Extensions provides an API that enables web applications to interact with content protection systems, to allow playback of encrypted audio and video. Provides access to the requestMediaKeySystemAccess() method, a part of the MediaKeys object. |
| execution-while-not-rendered    | Controls if tasks should execute for nested browsing contexts (eg. iframes) when it has/is not being rendered. |
| execution-while-out-of-viewport | Controls if tasks should execute for nested browsing contexts (eg. iframes) when not within the current viewport. |
| fullscreen                      | Determines whether any content in a document is allowed to go fullscreen. If disabled in any document, no content in the document will be allowed to use fullscreen. |
| geolocation                     | The Geolocation API provides access to geographical location information associated with the host device. |
| gyroscope                       | Gyroscope sensor interface to monitor the rate of rotation around the three local primary axes of the device. |
| keyboard-map                    | Controls whether the getLayoutMap() method is exposed on the "Keyboard" interface. |
| magnetometer                    | Magnetometer sensor interface to measure a magnetic field in the X, Y and Z axis. |
| microphone                      | Manages access to Microphone interfaces (physical and virtual). |
| midi                            | Musical Instrument Digital Interface (MIDI) protocol enables electronic musical instruments, controllers and computers to communicate and synchronize with each other. |
| navigation-override             | Enables the page author to take control over the behavior of spatial navigation, or to cancel it outright. Spatial navigation is the ability to move around the page directionally which can be useful for a web page built using a grid-like layout, or other predominantly non linear layouts. More often this is used in browsers on devices with limited input control, such as a TV. |
| payment                         | Allow merchants (i.e. web sites selling physical or digital goods) to utilise one or more payment methods with minimal integration. |
| picture-in-picture              | Allow websites to create a floating video window always on top of other windows so that users may continue consuming media while they interact with other content sites, or applications on their device. This item controls whether the request Picture-in-Picture algorithm may return a SecurityError and whether pictureInPictureEnabled is true or false. |
| publickey-credentials-get       | Determines whether any content in the allowed documents is allowed to successfully invoke the Web Authentication API. If disabled in any document, no content in the document will be allowed to use the foregoing methods, attempting to do so will return an error. |
| screen-wake-lock                | A screen wake lock prevents the screen from turning off. Only visible documents can acquire the screen wake lock. |
| sync-xhr                        | The sync-xhr policy controls whether synchronous requests can be made through the XMLHttpRequest API. If disallowed in a document, then calls to send() on XMLHttpRequest objects with the synchronous flag set will fail, causing a NetworkError DOMException to be thrown. |
| usb                             | The WebUSB API provides a way to safely expose USB device services to the web. Controls whether the usb attribute is exposed on the Navigator object. |
| web-share                       | Exposes the navigator.share() API where supported, which shares the current URL via user agent provided share to locations. |
| xr-spatial-tracking             | The WebXR Device API provides the interfaces necessary to enable developers to build compelling, comfortable, and safe immersive applications on the web across a wide variety of hardware form factors. |

**Proposed Features**

| Feature Name      | Description                                                  |
| ----------------- | ------------------------------------------------------------ |
| clipboard-read    | Read from the device clipboard via the Clipboard API         |
| clipboard-write   | Write to the device clipboard via the Clipboard API          |
| gamepad           | Determines whether any content in that document is allowed to access getGamepads(). If disabled in any document, no content in the document will be allowed to use getGamepads(), nor will the "gamepadconnected" and "gamepaddisconnected" events fire. |
| speaker-selection | Determines whether any content in a document is allowed to use the selectAudioOutput function to prompt the user to select an audio output device, or allowed to use setSinkId to change the device through which audio output should be rendered, to a non-system-default user-permitted device. |

**Experimental Features**

| Feature Name                  | Description                                                  |
| ----------------------------- | ------------------------------------------------------------ |
| conversion-measurement        | Click Through Attribution Reporting. To enable this, use the Chrome command line flag --enable-blink-features=ConversionMeasurement |
| focus-without-user-activation | Helps control the use of automated focus in a main frame or `<iframe>`. The proposed feature provides a means for developers to block the use of automatic focus in nested contents. |
| hid                           | Allow a web page to communicate with HID devices (Human Interface Device) |
| idle-detection                | Allow usage of the IdleDetector interface to better detect if a user is at their device, instead of trying to identify if a user has just become inactive, such as left window open, screen saver activated, screen turned off, changed tabs or changed applications. |
| interest-cohort               | Federated Learning of Cohorts (FLoC) is a new way that browsers could enable interest-based advertising on the web. A site should be able to declare that it does not want to be included in the user's list of sites for cohort calculation. |
| serial                        | Provide direct communication between a web site and the device that it is controlling via a Serial port. To enable this, use the Chrome command line flag --enable-blink-features=Serial |
| sync-script                   | Unknown - No information currently available. To enable this, use the Chrome command line flag --enable-blink-features=ExperimentalProductivityFeatures. |
| trust-token-redemption        | This API proposes a new per-origin storage area for “Privacy Pass” style cryptographic tokens, which are accessible in third party contexts. These tokens are non-personalized and cannot be used to track users, but are cryptographically signed so they cannot be forged. |
| window-placement              | Proposal to provide additional informatiion for Multi-Screen Window Placement. |
| vertical-scroll               | Vertical scroll policy is a feature introduced to assist websites in blocking certain embedded contents from interfering with vertical scrolling. Stopping a user from vertically scrolling the page might be a frustrating experience. |




### 5.1 Customize Permissions Policy

Customize Permissions Policy in HTTP Header


```cs
    public class Startup
    {
        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseDefaultHeaders(h=> {
                var policy = new PermissionsPolicy();
                
                policy.Feature(FeatureNames.Camera).Disable();
            	policy.Feature(FeatureNames.Fullscreen).Enable();
            	policy.Feature(FeatureNames.Geolocation).Enable().Self();
            	policy.Feature(FeatureNames.Usb).Enable().Custom("https://www.dnv.com");
            	policy.Feature(FeatureNames.Microphone).Enable().Self().Custom("https://www.google.com");
                
                h.Add(PermissionsPolicy.Key, policy.ToString());
            });
            //...
        }
    }
```

### 5.2 Enable all Permissions Policy

Enable all Permissions Policy in HTTP Header
```cs
    public class Startup
    {
        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseDefaultHeaders(h=> h.EnableAllPermissionsPolicyForSelf());
            //...
        }
    }
```

### 5.3 Disable all Permissions Policy

Disable all Permissions Policy in HTTP Header

```cs
    public class Startup
    {
        //...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...
            app.UseDefaultHeaders(h=> h.DisableAllPermissionsPolicy());
            //...
        }
    }
```
