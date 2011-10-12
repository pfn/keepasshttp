load('lib/sjcl.js');

function print(s) { java.lang.System.out.print(s) }
function println(s) { java.lang.System.out.println(s) }

function Lock() {
    this.waitMethod = this.class.getMethod("wait", null);
    this.notifyMethod = this.class.getMethod("notify", null);
}
Lock.prototype = {
    get class() {
        return java.lang.Class.forName("java.lang.Object");
    },
    done: false,

    wait: sync(function() {
        if (this.done) {
            this.done = false;
            return;
        }
        this.waitMethod.invoke(this, null);
    }),
    notify: sync(function() {
        this.done = true;
        this.notifyMethod.invoke(this, null);
    }),
};

var xhr = {
    request: function(method, request, callback) {
             var xhr = new XMLHttpRequest();
             var onchange = function() {
                 if (xhr.readyState == 4) {
                     lock.notify();
                     callback(xhr.responseText);
                 }
             };
             //print("Request: " + request);
             xhr.onreadystatechange = onchange;
             xhr.open(method, "http://localhost:19455", true);
             xhr.send(request);
             lock.wait();
             assert_status(xhr.status);
         },
    post: function(request, callback) {
             xhr.request("POST", request, callback);
         },
    success: function(s) {
                 return s >= 200 && s <= 299;
         }
};

function generate_iv() {
    var iv = [];
    for (var i = 0; i < 16; i++) {
        iv.push(String.fromCharCode(Math.floor(Math.random() * 256)));
    }
    iv = iv.join('');
    return btoa(iv);
}

function set_verifier(request) {
    var iv = generate_iv();
    request.Id = name;
    request.Nonce = iv;
    request.Verifier = encrypt(iv, iv);
}

function assert_status(status) {
    if (!xhr.success(status)) {
        throw new Error("Bad status: " + status);
    }
}

function assert_success(response) {
    var r = JSON.parse(response);
    if (!r.Success)
        throw new Error("Non-successful response");
}

function assert_equals(a, b) {
    if (a !== b)
        throw new Error("Expected: [" + a + "] got [" + b + "]");
}


function skiptest(tfn) { println("Skipping test: " + tfn.name } // noop
var test = (function() {
    var _tests = [];
    return function(tfn) {
        var failures = 0;
        if (tfn)
            _tests.push(tfn);
        else {
            for (var i in _tests) {
                print("Test: " + _tests[i].name);
                try {
                    _tests[i]();
                    println(" -- OK");
                } catch (e) {
                    println(" -- failed: " + e);
                    failures++;
                }
            }
            if (failures) {
                console.log("\nFailed %d/%d tests", failures, _tests.length);
            }
            else {
                println("\nAll tests passed");
            }
            return failures == 0;
        }
    }
})();

function get_logins(url, submiturl, realm, cb) {
    var request = {
        RequestType: "get-logins",
    };
    set_verifier(request);
    var iv = request.Nonce;
    request.Url = encrypt(url, iv);
    if (submiturl)
        request.SubmitUrl = encrypt(submiturl, iv);
    if (realm)
        request.Realm = encrypt(realm, iv);
    xhr.post(JSON.stringify(request), function(r) {
        cb(r);
    });
}

function encrypt(data, iv) {
    var enc = sjcl.mode.cbc.encrypt(
            new sjcl.cipher.aes(sjcl.codec.base64.toBits(key)),
            sjcl.codec.utf8String.toBits(data),
            sjcl.codec.base64.toBits(iv));
    return sjcl.codec.base64.fromBits(enc);
}

function decrypt(data, iv) {
    var dec = sjcl.mode.cbc.decrypt(
            new sjcl.cipher.aes(sjcl.codec.base64.toBits(key)),
            sjcl.codec.base64.toBits(data),
            sjcl.codec.base64.toBits(iv));
    return sjcl.codec.utf8String.fromBits(dec);
}
