# KeePassHttp

is a plugin for KeePass 2.x and provides a secure means of exposing KeePass entries via HTTP for clients to
consume.

This plugin is primarily intended for use with [PassIFox for Mozilla Firefox and chromeIPass for Google Chrome](https://github.com/pfn/passifox/).

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

## Installation
 1. Download [KeePassHttp](https://raw.github.com/pfn/keepasshttp/master/KeePassHttp.plgx)
 2. Copy it into the KeePass directory
 3. On linux systems you maybe need to install mono-complete: `$ apt-get install mono-complete`
 4. Restart KeePass

### KeePassHttp on Linux and Mac

KeePass needs Mono. You can find detailed [installation insructions on the official page of KeePass](http://keepass.info/help/v2/setup.html#mono).

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
Nevertheless, until KeePass v2.21 I used the 2 suggested .dll's and it
worked fine too.

Usually I only use chromeIPass, but I did a short test
with PassIFox and seems to be working just fine.

## Configuration and Options

KeePassHttp works out-of-the-box. You don't have to explicitely configure it.

 * KeePassHttp stores shared AES encryption keys in "KeePassHttp Settings" in the root group of a password database.
 * Password entries saved by KeePassHttp are stored in a new group named "KeePassHttp Passwords" within the password database.
 * Remembered Allow/Deny settings are stored as JSON in custom string fields within the individual password entry in the database.

### Settings in KeePassHttp options.

You can open the options dialog with menu: Tools > KeePassHttp Options...

[<img src="https://raw.github.com/pfn/keepasshttp/master/documentation/images/menu.jpg" alt="menu" width="300px" />](https://raw.github.com/pfn/keepasshttp/master/documentation/images/menu.jpg)

The options dialog will appear:

[<img src="https://raw.github.com/pfn/keepasshttp/master/documentation/images/options-general.png" alt="options-general" width="300px" />](https://raw.github.com/pfn/keepasshttp/master/documentation/images/options-general.png)

1. show a notification balloon whenever entries are delivered to the inquirer.
2. returns only the best matching entries for the given url, otherwise all entries for a domain are send
  - e.g. of two entries with the URLs http://example.org and http://example.org/, only the second one will returned if the requested URL is http://example.org/index.html
3. if the active database in KeePass is locked, KeePassHttp sends a request to unlock the database. Now KeePass opens and the user has to enter the master password to unlock the database. Otherwise KeePassHttp tells the inquirer that the database is closed.
4. KeePassHttp returns only these entries which match the scheme of the given URL
  - given URL: https://example.org --> scheme: https:// --> only entries whose URL starts with https://
5. removes all shared encryption-keys which are stored in the currently selected database. Every inquirer has to reauthenticate.
6. removes all stored permissions in the entries of the currently selected database.
7. Sort found entries by username or title  
  .  
  [<img src="https://raw.github.com/pfn/keepasshttp/master/documentation/images/options-advanced.png" alt="options-advanced" width="300px" />](https://raw.github.com/pfn/keepasshttp/master/documentation/images/options-advanced.png)
8. KeePassHttp no longer asks for permissions to retrieve entries, it always allows the access.
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

1. The host of the URL is extracted (http://www.example.org --> www.example.org)
2. All entries in the database are searched for this host:
  - searched fields: title + URL
  - matching result will be: at least of the searched fields contain the host ("http://www.example.org/index.html" contains "www.example.org")
  - if no entry was found, the host is cropped from beginning to the next point and the search restarts (www.example.org --> example.org --> org).
3. Now all found entries are filtered for one of the following points:
  - title-field starts with http://, https://, ftp:// or sftp:// and the parsed hostname of the title-field is contained in the host of (1)
  - URL-field starts with http://, https://, ftp:// or sftp:// and the parsed hostname of the title-field is contained in the host of (1)
  - host of (1) contains the whole content of the title-field or the URL-field
4. If the request passed the flag _SortSelection_ the filtered entries are sorted by best matching URL:
  - entryURL from URL-field of an entry is prepared
     - ending slash will be removed
     - missing scheme in front of entryURL will add "http://" to the beginning of entryURL
  - baseSubmitURL is the submitURL without ending filename and arguments (http://www.example.org/index.php?arg=1 --> http://www.example.org)
  - baseEntryURL is the entryURL without ending filename and arguments
  - sort order by highest matching:
     1. submitURL == entryURL
     2. submitURL starts with entryURL and entryURL != host (1) and baseSubmitURL != entryURL
     3. submitURL starts with baseEntryURL and entryURL != host (1) and baseSubmitURL != baseEntryURL
     4. entryURL == host (1)
     5. entryURL == baseSubmitURL
     6. entryURL starts with submitURL
     7. entryURL starts with baseSubmitURL and baseSubmitURL != host (1)
     8. submitURL starts with entryURL
     9. submitURL starts with baseEntryURL
     10. entryURL starts with host (1)
     11. host (1) starts with entryURL
     12. otherwise last position in list
5. If the setting for best matching entries is activated only the entries with the highest matching of step 4 will be returned.
 
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
