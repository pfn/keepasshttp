# KeePassHttp

is a plugin for KeePass and provides a secure means of exposing KeePass entries via HTTP for clients to
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

## Installation
 1. Download [KeePassHttp](https://raw.github.com/pfn/keepasshttp/master/KeePassHttp.plgx)
 2. Copy it into the KeePass directory
 3. On linux systems you maybe need to install mono-complete:
    $ apt-get install mono-complete
 4. Restart KeePass

## System requirements
 * KeePass 2.17 or higher
 * For Windows: Windows XP SP3 or higher
 * For Linux: installed mono
 * For Mac: installed mono | it seems to fully support KeePassHttp, but we cannot test it

## Configuration and Options

KeePassHttp works out-of-the-box. You don't have to explicitely configure it.

 * KeePassHttp stores shared AES encryption keys in "KeePassHttp Settings" in the root group of a password database.
 * Password entries saved by KeePassHttp are stored in a new group named "KeePassHttp Passwords" within the password database.
 * Remembered Allow/Deny settings are stored as JSON in custom string fields within the individual password entry in the database.

### Settings in KeePassHttp options.

With Menu > Tools > KeePassHttp Options... you can open the options dialog.
![menu](https://raw.github.com/pfn/keepasshttp/documentation/images/master/menu.jpg)

This dialog will appear:
![menu](https://raw.github.com/pfn/keepasshttp/documentation/images/master/options.jpg)
1. show a notification balloon whenever entries are delivered to the inquirer.
2. returns only the best matching entries for the given url, otherwise all entries for a domain are send
        e.g. of two entries with the URLs http://example.org and http://example.org/, only the second one will returned if the requested URL is http://example.org/index.html
3. if the active database in KeePass is locked, KeePassHttp sends a request to unlock the database. Now KeePass opens and the user has to enter the master password to unlock the database. Otherwise KeePassHttp tells the inquirer that the database is closed.
4. KeePassHttp no longer asks for permissions to retrieve entries, it always allows the access.
5. KeePassHttp no longer asks for permission to update an entry, it always allows updating them.
6. Searching for entries is no longer restricted to the current active database in KeePass but is extended to all opened databases!
      __Important:__ Even if a second database is not connected with the inquirer, KeePassHttp will search and retrieve entries of all opened databases!
7. Removes all stored connection-keys. Every inquirer has to reauthenticate.

## Troubleshooting

If an error occures it will be shown as notification in system tray or as messagebox in KeePass.

If you are having problems with KeePassHttp, please tell us which versions of
* KeePass
* KeePassHttp
* error message (if availbale)
* clients
you are using.
 
## Security

For security reasons KeePassHttp communicates only with the symmetric-key algorithm AES.
The entries are crypted with a 256bit AES key.

There is one single point where someone else will be able to steal the encryption keys.
If a new client has to connect to KeePassHttp, the encryption key is generated and send to KeyPassHttp via an unencrypted connection.


## Compile at your own

If you want to develop new features or improve existing ones here is a way to build it at your own:
1. 
2. delete the directory "bin" from sourcecode
3. delete the directory "obj" from sourcecode
4. delete the file "KeePassHttp.dll"

I use the following batch code to automatically do steps 2 - 4:
	RD /S /Q C:\full-path-to-keepasshttp-source\bin
	RD /S /Q C:\full-path-to-keepasshttp-source\obj
	DEL C:\full-path-to-keepasshttp-source\KeePassHttp.dll
	"C:\Program Files (x86)\KeePass Password Safe 2\keepass.exe" --plgx-create p:\dev\C#\keepasshttp\KeePassHttp


## Protocol
1. New client or stale client (key not in database).  This is the only point at which an administrator snooping traffic will be able to steal encryption keys:

   a) client sends "test-associate" with payload to server
   b) server sends fail response to client (cannot decrypt)
   c) client sends "associate" with 256bit AES key and payload to server
   d) server decrypts payload with provided key and prompts user to save
   e) server saves key into "KeePassHttpSettings":"AES key: label"
   f) client saves label/key into local password storage

	  (a) can be skipped if client does not have a key configured

2. Client with key stored in server
   a) client sends "test-associate" with label+encrypted payload to server
   b) server verifies payload and responds with success to client
   c) client sends any of "get-logins-count", "get-logins", "set-login"
	  using the previously negotiated key in (1)
   d) if any subsequent request fails, it is necessary to "test-associate"
	  again