# KeePassHttp

is a plugin for KeePass 2.x and provides a secure means of exposing KeePass entries via HTTP for clients to
consume.

This plugin is primarily intended for use with [PassIFox for Mozilla Firefox](https://github.com/pfn/passifox/) and [chromeIPass for Google Chrome](https://chrome.google.com/webstore/detail/chromeipass/ompiailgknfdndiefoaoiligalphfdae?hl=en).

## Features
 * returns all matching entries for a given URL
 * updates entries
 * secure exchange of entries
 * notifies user if entries are delivered
 * user can allow or deny access to single entries
 * works only if the database is unlocked
 * request for unlocking the database if it is locked while connecting
 * searches in all opened databases (if user activates this feature)
 * Whenever events occur, the user is prompted either by tray notification or requesting interaction (allow/deny/remember).

## System requirements
 * KeePass 2.17 or higher
 * For Windows: Windows XP SP3 or higher
 * For Linux: installed mono
 * For Mac: installed mono | it seems to fully support KeePassHttp, but we cannot test it

## Windows installation using Chocolatey

 1. Install using [Chocolatey](https://chocolatey.org/) with `choco install keepass-keepasshttp`
 2. Restart KeePass if it is currently running to load the plugin

## Non-Windows / Manual Windows installation

 1. Download [KeePassHttp](https://raw.github.com/pfn/keepasshttp/master/KeePassHttp.plgx)
 2. Copy it into the KeePass directory
	* default directory in Ubuntu14.04: /usr/lib/keepass2/
	* default directory in Arch: /usr/share/keepass
 3. Set chmod 644 on file `KeePassHttp.plgx`
 4. On linux systems you maybe need to install mono-complete: `$ apt-get install mono-complete` (in Debian it should be enough to install the packages libmono-system-runtime-serialization4.0-cil and libmono-posix2.0-cil)
 * Tips to run KeePassHttp on lastest KeePass 2.31: install packages
 	`sudo apt-get install libmono-system-xml-linq4.0-cil libmono-system-data-datasetextensions4.0-cil libmono-system-runtime-serialization4.0-cil mono-mcs`
 5. Restart KeePass

### KeePassHttp on Linux and Mac

KeePass needs Mono. You can find detailed [installation instructions on the official page of KeePass](http://keepass.info/help/v2/setup.html#mono).

Perry has tested KeePassHttp with Mono 2.6.7 and it appears to work well.
With Mono 2.6.7 and a version of KeePass lower than 2.20 he could not get the plgx file to work on linux.
If the plgx file does also not work for you, you can try the two DLL files KeePassHttp.dll and Newtonsoft.Json.dll from directory [mono](https://github.com/pfn/keepasshttp/tree/master/mono) which should work for you.

With newer versions of Mono and KeePass it seems that the plgx file works pretty fine.
More information about it are contained in the following experience report.

#### Experience report by dunkelfuerst
Just wanted to let you know, I'm running Fedora 18, which currently uses
mono v2.10.8:
~~~
> mono-core.x86_64                       2.10.8-3.fc18                     @fedora
> mono-data.x86_64                       2.10.8-3.fc18                     @fedora
> mono-data-sqlite.x86_64                2.10.8-3.fc18                     @fedora
> mono-extras.x86_64                     2.10.8-3.fc18                     @fedora
> mono-mvc.x86_64                        2.10.8-3.fc18                     @fedora
> mono-wcf.x86_64                        2.10.8-3.fc18                     @fedora
> mono-web.x86_64                        2.10.8-3.fc18                     @fedora
> mono-winforms.x86_64                   2.10.8-3.fc18                     @fedora
> mono-winfx.x86_64                      2.10.8-3.fc18                     @fedora
~~~

I have no problems using "KeePassHttp.plgx". I simply dropped the .plgx-file in my KeePass folder, and it works.

I'm currently using KeePass v2.22.  
Nevertheless, until KeePass v2.21 I used the 2 suggested .dll's and it worked fine too.

Usually I only use chromeIPass, but I did a short test
with PassIFox and seems to be working just fine.

## Configuration and Options

KeePassHttp works out-of-the-box. You don't have to explicitely configure it.

 * KeePassHttp stores shared AES encryption keys in "KeePassHttp Settings" in the root group of a password database.
 * Password entries saved by KeePassHttp are stored in a new group named "KeePassHttp Passwords" within the password database.
 * Remembered Allow/Deny settings are stored as JSON in custom string fields within the individual password entry in the database.

### Settings in KeePassHttp options.

You can open the options dialog with menu: Tools > KeePassHttp Options

[<img src="https://raw.github.com/pfn/keepasshttp/master/documentation/images/menu.jpg" alt="menu" width="300px" />](https://raw.github.com/pfn/keepasshttp/master/documentation/images/menu.jpg)

The options dialog will appear:

[<img src="https://raw.github.com/pfn/keepasshttp/master/documentation/images/options-general.png" alt="options-general" width="300px" />](https://raw.github.com/pfn/keepasshttp/master/documentation/images/options-general.png)

General tab

1. show a notification balloon whenever entries are delivered to the inquirer.
2. returns only the best matching entries for the given url, otherwise all entries for a domain are send.
  - e.g. of two entries with the URLs http://example.org and http://example.org/, only the second one will returned if the requested URL is http://example.org/index.html
3. if the active database in KeePass is locked, KeePassHttp sends a request to unlock the database. Now KeePass opens and the user has to enter the master password to unlock the database. Otherwise KeePassHttp tells the inquirer that the database is closed.
4. KeePassHttp returns only these entries which match the scheme of the given URL.
  - given URL: https://example.org --> scheme: https:// --> only entries whose URL starts with https://
5. sort found entries by username or title.
6. removes all shared encryption-keys which are stored in the currently selected database. Every inquirer has to reauthenticate.
7. removes all stored permissions in the entries of the currently selected database.

[<img src="https://raw.github.com/pfn/keepasshttp/master/documentation/images/options-advanced.png" alt="options-advanced" width="300px" />](https://raw.github.com/pfn/keepasshttp/master/documentation/images/options-advanced.png)

Advanced tab

8. KeePassHttp no longer asks for permissions to retrieve entries, it always allows access.
9. KeePassHttp no longer asks for permission to update an entry, it always allows updating them.
10. Searching for entries is no longer restricted to the current active database in KeePass but is extended to all opened databases!
  - __Important:__ Even if another database is not connected with the inquirer, KeePassHttp will search and retrieve entries of all opened databases if the active one is connected to KeePassHttp!
11. if activated KeePassHttp also search for string fields which are defined in the found entries and start with "KPH: " (note the space after colon). __The string fields will be transfered to the client in alphabetical order__. You can set string fields in the tab _Advanced_ of an entry.  
[<img src="https://raw.github.com/pfn/keepasshttp/master/documentation/images/advanced-string-fields.png" alt="advanced tab of an entry" width="300px" />](https://raw.github.com/pfn/keepasshttp/master/documentation/images/advanced-string-fields.png)

## Tips and Tricks

### Support multiple URLs for one username + password
This is already implemented directly in KeePass.

1. Open the context menu of an entry by clicking right on it and select _Duplicate entry_:  
[<img src="https://raw.github.com/pfn/keepasshttp/master/documentation/images/keepass-context-menu.png" alt="context-menu-entry" />](https://raw.github.com/pfn/keepasshttp/master/documentation/images/keepass-context-menu.png)

2. Check the option to use references for username and password:  
[<img src="https://raw.github.com/pfn/keepasshttp/master/documentation/images/keepass-duplicate-entry-references.png" alt="mark checkbox references" width="300px" />](https://raw.github.com/pfn/keepasshttp/master/documentation/images/keepass-duplicate-entry-references.png)

3. You can change the title, URL and evertything of the copied entry, but not the username and password. These fields contain a _Reference Key_ which refers to the _master entry_ you copied from.

## Troubleshooting

__First:__ If an error occures it will be shown as notification in system tray or as message box in KeePass.

Otherwise please check if it could be an error of the client you are using. For passIFox and chromeIPass you can [report an error here](https://github.com/pfn/passifox/issues/).


If you are having problems with KeePassHttp, please tell us at least the following information:
* version of KeePass
* version of KeePassHttp
* error message (if available)
* used clients and their versions
* URLs on which the problem occur (if available)

### HTTP Listener error message

Maybe you get the following error message:  
[<img src="https://raw.github.com/pfn/keepasshttp/master/documentation/images/http-listener-error.png" alt="http listener error" width="300px" />](https://raw.github.com/pfn/keepasshttp/master/documentation/images/http-listener-error.png)

In old versions the explaining first part of the message does not exist!

This error occurs because you have multiple copies of KeePassHttp in your KeePass directory! Please check __all__ PLGX- and DLL-files in your _KeePass directory and all sub-directories_ whether they are a copy of KeePassHttp.  
__Note:__ KeePass does _not_ detect plugins by filename but by extension! If you rename _KeePassHttp.plgx_ to _HelloWorld.plgx_ it is still a valid copy of KeePassHttp.

If you _really_ have only one copy of KeePassHttp in your KeePass directory another application seems to use port 19455 to wait for signals. In this case try to stop all applications and restart everyone again while checking if the error still occurs.

## URL matching: How does it work?

KeePassHttp can receive 2 different URLs, called URL and SubmitURL.

CompareToUrl = SubmitURL if set, URL otherwise

For every entry, the [Levenshtein Distance](http://en.wikipedia.org/wiki/Levenshtein_distance) of his Entry-URL (or Title, if Entry-URL is not set) to the CompareToURL is calculated.

Only the Entries with the minimal distance are returned.

###Example:
Submit-Url: http://www.host.com/subdomain1/login

Entry-URL|Distance
---|---
http://www.host.com/|16
http://www.host.com/subdomain1|6
http://www.host.com/subdomain2|7

__Result:__ second entry is returned

 
## Security

For security reasons KeePassHttp communicates only with the symmetric-key algorithm AES.
The entries are crypted with a 256bit AES key.

There is one single point where someone else will be able to steal the encryption keys.
If a new client has to connect to KeePassHttp, the encryption key is generated and send to KeyPassHttp via an unencrypted connection.


## Compile at your own

If you want to develop new features or improve existing ones here is a way to build it at your own:

1. copy the file [Newtonsoft.Json.dll](http://json.codeplex.com/releases/) into the sourcecode folder
2. delete the directory "bin" from sourcecode
3. delete the directory "obj" from sourcecode
4. delete the file "KeePassHttp.dll"

I use the following batch code to automatically do steps 2 - 4:

	RD /S /Q C:\full-path-to-keepasshttp-source\bin
	RD /S /Q C:\full-path-to-keepasshttp-source\obj
	DEL C:\full-path-to-keepasshttp-source\KeePassHttp.dll
	"C:\Program Files (x86)\KeePass Password Safe 2\keepass.exe" --plgx-create C:\full-path-to-keepasshttp-source


## Protocol

### A. New client or stale client (key not in database).

This is the only point at which an administrator snooping traffic will be able to steal encryption keys:

1. client sends "test-associate" with payload to server
2. server sends fail response to client (cannot decrypt)
3. client sends "associate" with 256bit AES key and payload to server
4. server decrypts payload with provided key and prompts user to save
5. server saves key into "KeePassHttpSettings":"AES key: label"
6. client saves label/key into local password storage

(1) can be skipped if client does not have a key configured

### B. Client with key stored in server
1. client sends "test-associate" with label+encrypted payload to server
2. server verifies payload and responds with success to client
3. client sends any of "get-logins-count", "get-logins", "set-login" using the previously negotiated key in (A)
4. if any subsequent request fails, it is necessary to "test-associate" again

## A little deeper into protocol

### Generic HTTP request
(based on packet sniffing and code analyssis)
Generic HTTP request is json sent in POST message. Cipher, by means of OpenSSL library is `AES-256-CBC`, so key is 32 byte long. 

```
Host: localhost:19455
Connection: keep-alive
Content-Length: 54
Content-Type: application/json
Accept: */*
Accept-Encoding: gzip, deflate, br

{"RequestType":"test-associate","TriggerUnlock":false}

```

Also, minimal JSON request (except that one without key set up) consists of four main parameters:
 - RequestType - `test-associate`, `associate`, `get-logins`, `get-logins-count`, `set-login`, ...
 - TriggerUnlock - TODO: what is this good for? seems always false
 - Nonce - 128 bit (16 bytes) long random vector, base64 encoded, used as IV for aes encryption
 - Verifier - verifier, base64 encoded AES encrypted data: `encrypt(base64_encode($nonce), $key, $nonce);`
 - Id - Key id entered into KeePass GUI while  `associate`, not used during `associate`

### test-associate
Request, without key, seems like initialization of every key assignation session:
```javascript
{
    "RequestType":"test-associate",
    "TriggerUnlock":false
}
``` 

Response: (without success)
```javascript
{
    "Count":null,
    "Entries":null,
    "Error":"",
    "Hash":"d8312a59523d3c37d6a5401d3cfddd077e194680",
    "Id":"",
    "Nonce":"",
    "RequestType":"test-associate",
    "Success":false,
    "Verifier":"",
    "Version":"1.8.4.1",
    "objectName":""
}
```

If you have key, you can test with request like this: 
```javascript
{
    "Nonce":"+bG+EpbCR4jSnjROKAAw4A==", // random 128bit vector, base64 encoded
    "Verifier":"2nVUxyddGpe62WGx5cm3hcb604Xn8AXrYxUK2WP9dU0=", // Nonce in base64 form, encoded with aes
    "RequestType":"test-associate",
    "TriggerUnlock":false,
    "Id":"PHP"
}
```

### associate
Request:
```javascript
{
    "RequestType":"associate",
    "Key":"CRyXRbH9vBkdPrkdm52S3bTG2rGtnYuyJttk/mlJ15g=", // Base64 encoded 256 bit key
    "Nonce":"epIt2nuAZbHt5JgEsxolWg==",
    "Verifier":"Lj+3N58jkjoxS2zNRmTpeQ4g065OlFfJsHNQWYaOJto="
}
```

Response:
```javascript
{
    "Count":null,
    "Entries":null,
    "Error":"",
    "Hash":"d8312a59523d3c37d6a5401d3cfddd077e194680",
    "Id":"PHP", // You need to save this - to use in future
    "Nonce":"cJUFe18NSThQ/0yAqZMaDA==",
    "RequestType":"associate",
    "Success":true,
    "Verifier":"ChH0PtuQWP4UKTPhdP3XSgwFyVdekHmHT7YdL1EKA+A=",
    "Version":"1.8.4.1",
    "objectName":""
}
```

### get-logins

Request:
```javascript
{
    "RequestType":"get-logins",
    "SortSelection":"true",
    "TriggerUnlock":"false",
    "Id":"PHP",
    "Nonce":"vCysO8UwsWyE2b+nMzE3/Q==",
    "Verifier":"5Nyi5973GawqdP3qF9QlAF/KlZAyvb6c5Smhun8n9wA=",
    "Url":"Gz+ZCSjHAGmeYdrtS78hSxH3yD5LiYidSq9n+8TdQXc=", // Encrypted URL
    "SubmitUrl":"<snip>" // Encrypted submit URL
}
```

Response:
```javascript
{
    "Count":3,
    "Entries":[
        {
            "Login":"{encrypted login base64}",
            "Name":"{encrypted item name}",
            "Password":"{encrypted Password}",
            "StringFields":null,
            "Uuid":"{encrypted UUID}"
        },
        {
            <snip>
        },
        {
            <snip>
        }
    ],
    "Error":"",
    "Hash":"d8312a59523d3c37d6a5401d3cfddd077e194680",
    "Id":"PHP",
    "Nonce":"Aeh9maerCjE5v5V8Tz2YxA==",
    "RequestType":"get-logins",
    "Success":true,
    "Verifier":"F87c4ggkMTSEptJT8/FypBH491kRexTAiEZxovLMvD8=",
    "Version":"1.8.4.1",
    "objectName":""
}

```

### get-logins-count

Request:
```javascript
{
    "RequestType":"get-logins-count",
    "TriggerUnlock":"false",
    "Id":"PHP",
    "Nonce":"vCysO8UwsWyE2b+nMzE3/Q==",
    "Verifier":"5Nyi5973GawqdP3qF9QlAF/KlZAyvb6c5Smhun8n9wA=",
    "Url":"Gz+ZCSjHAGmeYdrtS78hSxH3yD5LiYidSq9n+8TdQXc=", // Encrypted URL
    "SubmitUrl":"<snip>" // Encrypted submit URL
}
```

Response:
```javascript
{
    "Count":3,
    "Entries":null,
    "Error":"",
    "Hash":"d8312a59523d3c37d6a5401d3cfddd077e194680",
    "Id":"PHP",
    "Nonce":"Aeh9maerCjE5v5V8Tz2YxA==",
    "RequestType":"get-logins",
    "Success":true,
    "Verifier":"F87c4ggkMTSEptJT8/FypBH491kRexTAiEZxovLMvD8=",
    "Version":"1.8.4.1",
    "objectName":""
}
```

### set-login

Request:
```javascript
{
    "RequestType":"set-login",
    "Id":"PHP",
    "Nonce":"VBrPACEOQGxIBkq58/5Xig==",
    "Verifier":"1dT0gnw6I1emxDzhtYn1Ecn1sobLG98GfTf7Z/Ma0R0=",
    "Login":"lm9qo5HcAYEIaHsCdSsYHQ==", // encrypted username
    "Password":"EZLtRxFgZVqIwv5xI9tfvA==", // encrypted password
    "Url":"<snip>",
    "SubmitUrl":"<snip>"
}
```

Response:
```javascript
{
    "Count":null,
    "Entries":null,
    "Error":"",
    "Hash":"d8312a59523d3c37d6a5401d3cfddd077e194680",
    "Id":"PHP",
    "Nonce":"uofAcMtnPQo5TOdI21VjBw==",
    "RequestType":"set-login",
    "Success":true,
    "Verifier":"4u8OINVGBtlCCPY7OnW5T616iPlzvf56LzPtPAwZIs0=",
    "Version":"1.8.4.1",
    "objectName":""
}
```
