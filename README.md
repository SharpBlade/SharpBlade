Sharparam.SharpBlade
====================

[![TC Status](http://tc.sharpblade.net/app/rest/builds/buildType:%28id:sharpblade_mainbuild%29/statusIcon)](http://tc.sharpblade.net/viewType.html?buildTypeId=sharpblade_mainbuild&guest=1)

A C# wrapper/implementation for the SwitchBlade UI API
------------------------------------------------------

Built using the SwitchBlade UI SDK from Razer.

Contributing
------------

Contributors are very welcome! If you got code fixes, please [submit a pull request][newpull] here on GitHub.

If you want to join the development team, please contact [Sharparam][sharp] on GitHub.

All authors and contributors are listed in the **AUTHORS** file.

Please read the wiki page about [contributing][contrib] before submitting pull requests.

License
-------

Copyright &copy; 2013-2014 by [Adam Hellberg][sharp] and [Brandon Scott][bs].

This project is licensed under the MIT license, please see the file **LICENSE** for more information.

Images in **res/images** are created by Graham Hough.

Razer is a trademark and/or a registered trademark of Razer USA Ltd.  
All other trademarks are property of their respective owners.

This project is in no way endorsed, sponsored or approved by Razer.

Apache log4net Copyright 2004-2011 The Apache Software Foundation (Apache License 2.0).

Dependencies
------------

Sharparam.SharpBlade depends on the SwitchBlade UI SDK (RzSwitchbladeSDK2.dll).

SwitchBlade UI SDK is provided by Razer and [can be obtained from their website][rzdev].

Sharparam.SharpBlade depends on the [log4net][l4n] library (provided).

Debugging / Logging
-------------------

You may have to enable **Native Code Debugging** in the project settings to properly debug projects using SharpBlade.

SharpBlade outputs log information to the standard output stream by default, if it fails to detect any log4net config information in the default location (App.config).

If you want log4net to output to a file, you can put the following in your App.config file:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
  </configSections>
  <log4net>
    <appender name="FileAppender" type="log4net.Appender.FileAppender">
      <file value="app.log" /> <!-- Change filename if desired -->
      <appendToFile value="false" />
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level %logger [%property{NDC}] - %message%newline" />
      </layout>
    </appender>
    <root>
      <level value="INFO" /> <!-- Change "INFO" to "DEBUG" to make logging more verbose -->
      <appender-ref ref="FileAppender" />
    </root>
  </log4net>
</configuration>
```

(Note that if you already have stuff in your App.config, just add the `<log4net>` section and add the `<section />` tag to `<configSections>`)

This will cause log output to be saved in a file named "app.log" in the same directory as the executable.

Projects
--------

Current projects utilizing this or modified versions of this library:

 * [SwitchBladeSteam][sbs]
 * Razer Notes
 * Razer Default Audio
 * Razer Spotify

(If you want your project listed, just contact [Sharparam][sharp] or [Brandon][bs])

[newpull]: ../../pull/new/master
[sharp]: https://github.com/Sharparam
[contrib]: ../../wiki/Contributing
[bs]: https://github.com/brandonscott
[rzdev]: http://www.razerzone.com/switchblade-ui/developers
[l4n]: http://logging.apache.org/log4net/
[sbs]: https://github.com/Sharparam/SwitchBladeSteam
