var name = "Test Key";
var key = "Lgh8xMEkV2j10bG7O42GjCibsUEpM80T7Db+skKGiNc=";

var lock = new Lock();

test(function test_associate_ok() {
    var request = {
        RequestType: "test-associate",
    };
    set_verifier(request);
    var resp;
    xhr.post(JSON.stringify(request), function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    assert_success(resp);
});

test(function get_logins_empty() {
    var resp;
    get_logins("http://www.doesnotexist.com/", null, null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(0, response.Count);
});

test(function get_logins_match_partial_title() {
    var resp;
    get_logins("http://www.google.com/", null, null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(1, response.Entries.length);
    assert_equals("google-user", decrypt(response.Entries[0].Login, response.Nonce));
});

test(function get_logins_match_exact_title() {
    var resp;
    get_logins("http://www.yahoo.com/", null, null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(1, response.Entries.length);
    assert_equals("www.yahoo-user", decrypt(response.Entries[0].Login, response.Nonce));
});

test(function get_logins_match_partial_title_yahoo() {
    var resp;
    get_logins("http://yahoo.com/", null, null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(1, response.Entries.length);
    assert_equals("yahoo-user", decrypt(response.Entries[0].Login, response.Nonce));
});

test(function get_logins_match_host_urlfield() {
    var resp;
    get_logins("http://citi.com/", null, null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(1, response.Entries.length);
    assert_equals("citi-user", decrypt(response.Entries[0].Login, response.Nonce));
});

test(function get_logins_match_real_urlfield() {
    var resp;
    get_logins("http://citi1.com/", null, null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(1, response.Entries.length);
    assert_equals("citi1-user", decrypt(response.Entries[0].Login, response.Nonce));
});

test(function get_logins_match_title_and_urlfield() {
    var resp;
    get_logins("https://cititest.com/", null, null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(1, response.Entries.length);
    assert_equals("cititest-user", decrypt(response.Entries[0].Login, response.Nonce));
});

test(function get_logins_match_title_urltitle_mismatch() {
    var resp;
    get_logins("https://bogustest.com/", null, null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(1, response.Entries.length);
    assert_equals("bogustest-user", decrypt(response.Entries[0].Login, response.Nonce));
});

test(function get_logins_match_url_urltitle_mismatch() {
    var resp;
    get_logins("https://www.bogustest.com/", null, null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(1, response.Entries.length);
    assert_equals("bogustest-user", decrypt(response.Entries[0].Login, response.Nonce));
});

test(function get_logins_match_title_urltitle_mismatch2() {
    var resp;
    get_logins("https://bogustest1.com/", null, null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(1, response.Entries.length);
    assert_equals("bogustest1-user", decrypt(response.Entries[0].Login, response.Nonce));
});

test(function get_logins_match_url_urltitle_mismatch2() {
    var resp;
    get_logins("https://www.bogustest1.com/", null, null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(1, response.Entries.length);
    assert_equals("bogustest1-user", decrypt(response.Entries[0].Login, response.Nonce));
});

test(function get_logins_subpath() {
    var resp;
    get_logins("http://www.host.com", "http://www.host.com/path1", null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(1, response.Entries.length);
    assert_equals("user1", decrypt(response.Entries[0].Login, response.Nonce));
});
test(function get_logins_subpath_2() {
    var resp;
    get_logins("http://www.host.com", "http://www.host.com/path2?param=value", null, function(r) {
        resp = r;
        lock.notify();
    });
    lock.wait();
    var response = JSON.parse(resp);
    assert_equals(1, response.Entries.length);
    assert_equals("user2", decrypt(response.Entries[0].Login, response.Nonce));
});
