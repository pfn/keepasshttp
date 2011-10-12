// To test, load up test.kdbx into KeePass,
// the password is 'test' without quotes

// be sure to have KeePassHttp installed and minimally working first

// disable optimizations because envjs/rhino overruns them...
Packages.org.mozilla.javascript.Context.getCurrentContext()
        .setOptimizationLevel(-1);

load('lib/env.rhino.1.2.js');
load('lib/test-harness.js');
load('tests/test-kph.js');

test();
