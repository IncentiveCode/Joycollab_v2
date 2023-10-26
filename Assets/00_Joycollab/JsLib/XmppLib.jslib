var XmppLib = {

	psXmppLogin : function(gameObjectNamePtr, urlPtr, callbackMethodNamePtr) {
		var gameObjectName = UTF8ToString(gameObjectNamePtr);
		var url = UTF8ToString(urlPtr);
		var callbackMethodName = UTF8ToString(callbackMethodNamePtr);

		var tempInput = document.getElementById('unity-xmpp-msg');
		if (tempInput) {
			document.body.removeChild(tempInput);
		}
		
		var tempInput = document.createElement('input');
		tempInput.setAttribute('id', 'unity-xmpp-msg');
		tempInput.setAttribute('type', 'hidden');
		tempInput.onchange = function () {
			SendMessage(gameObjectName, callbackMethodName, tempInput.value);
		}
        document.body.appendChild(tempInput);
		
		var iframe = document.getElementById('unity-xmpp');
		iframe.src = url;
	},

	psXmppLogout : function() {
		var tempInput = document.getElementById('unity-xmpp-msg');
		if (tempInput) {
			document.body.removeChild(tempInput);
		}

		var iframe = document.getElementById('unity-xmpp');
		iframe.src = '';
	},

	psXmppRefresh : function() {
		var iframe = document.getElementById('unity-xmpp');
		iframe.contentWindow.location.reload();
	},

};

mergeInto(LibraryManager.library, XmppLib);