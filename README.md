# EaglesoftAES

This Proof of Concept shows how someone can create their own keyfile.cfg file. It does this 3 ways, one decrypts a file to find the license, which is the biggest vulnerability in Eaglesoft's new AES 256 encryption. All the keyfile does is append a switch to the commandline that starts a service. You could just specify it in the service commandline. You could also create a keyfile that is not machine specific.
