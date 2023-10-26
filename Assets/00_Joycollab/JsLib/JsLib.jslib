var JsLib = {

	psGetCookie : function(namePtr) {
		var ret = '';
		var name = UTF8ToString(namePtr) + '=';
		var decodedCookie = decodeURIComponent(document.cookie);
		var ca = decodedCookie.split(';');
		
		for(var i = 0; i < ca.length; i++) {
			var c = ca[i];
			while (c.charAt(0) == ' ') {
				c = c.substring(1);
			}
			if (c.indexOf(name) == 0) {
				ret = c.substring(name.length, c.length);
				break;
			}
		}

		var bufferSize = lengthBytesUTF8(ret) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(ret, buffer, bufferSize);
		return buffer;
	},

	psSetCookie : function(namePtr, valuePtr) {
		var d = new Date();
		d.setTime(d.getTime() + (360 * 24 * 60 * 60 * 1000));
		var expires = 'expires='+ d.toUTCString();
		document.cookie = UTF8ToString(namePtr) +'='+ UTF8ToString(valuePtr) +';'+ expires +'; path=/';
	},

	psAlert : function(messagePtr) {
		window.alert(UTF8ToString(messagePtr));
	},

	psLog : function(messagePtr) {
		console.log(UTF8ToString(messagePtr));
	},

	psRedirection : function(urlPtr) {
		window.location.href = UTF8ToString(urlPtr);
	},

	psOpenWebview : function(urlPtr, idPtr) {
		window.open(UTF8ToString(urlPtr), UTF8ToString(idPtr));
	},

	psCheckBrowser : function(gameObjectNamePtr, methodNamePtr) {
		var gameObjectName = UTF8ToString(gameObjectNamePtr);
		var methodName = UTF8ToString(methodNamePtr);

		var language = navigator.language;
		if (language != null) {
			language = language.toLowerCase().substring(0, 2);
			if (language == 'ko') 		language = 'ko';
			else if (language == 'ja')	language = 'ja';
			else 						language = 'en';
		}
		else {
			language = 'en';
		}

		var userAgent = navigator.userAgent.toLowerCase();
		if (userAgent.indexOf('chrome') !== -1) {
			var result = 'true|'+ language;
		}
		else {
			var result = 'false|'+ language;
		}

		SendMessage(gameObjectName, methodName, result);
	},

	psCheckSystem : function(gameObjectNamePtr, methodNamePtr) {
		var gameObjectName = UTF8ToString(gameObjectNamePtr);
		var methodName = UTF8ToString(methodNamePtr);

		// os, user agent check
		var os, ua = navigator.userAgent;
		if (ua.match(/Win(dows )?NT 6\.0/)) {
        	os = 'Windows Vista';
    	} else if (ua.match(/Win(dows )?(NT 5\.1|XP)/)) {
        	os = 'Windows XP';
    	} else {
			if ((ua.indexOf('Windows NT 5.1') != -1) || (ua.indexOf('Windows XP') != -1)) {
				os = 'Windows XP';
			} else if ((ua.indexOf('Windows NT 7.0') != -1) || (ua.indexOf('Windows NT 6.1') != -1)) {
				os = 'Windows 7';
			} else if ((ua.indexOf('Windows NT 8.0') != -1) || (ua.indexOf('Windows NT 6.2') != -1)) {
				os = 'Windows 8';
			} else if ((ua.indexOf('Windows NT 8.1') != -1) || (ua.indexOf('Windows NT 6.3') != -1)) {
				os = 'Windows 8.1';
			} else if ((ua.indexOf('Windows NT 10.0') != -1) || (ua.indexOf('Windows NT 6.4') != -1)) {
				os = 'Windows 10';
			} else if ((ua.indexOf('iPad') != -1) || (ua.indexOf('iPhone') != -1) || (ua.indexOf('iPod') != -1)) {
				os = 'Apple iOS';
			} else if (ua.indexOf('Android') != -1) {
				os = 'Android OS';
			} else if (ua.match(/Win(dows )?NT( 4\.0)?/)) {
				os = 'Windows NT';
			} else if (ua.match(/Mac|PPC/)) {
				os = 'Mac OS';
			} else if (ua.match(/Linux/)) {
				os = 'Linux';
			} else if (ua.match(/(Free|Net|Open)BSD/)) {
				os = RegExp.$1 + 'BSD';
			} else if (ua.match(/SunOS/)) {
				os = 'Solaris';
			}
		}
		if (os.indexOf('Windows') != -1) {
			if (navigator.userAgent.indexOf('WOW64') > -1 || navigator.userAgent.indexOf('Win64') > -1) {
				os += ' 64bit';
			} else {
				os += ' 32bit';
			}
		}	

		SendMessage(gameObjectName, methodName, os);
	},

	psRunScheme : function(gameObjectNamePtr, urlPtr, methodNamePtr) {
		var gameObjectName = UTF8ToString(gameObjectNamePtr);
		var url = UTF8ToString(urlPtr);
		var methodName = UTF8ToString(methodNamePtr);

		var chk500 = false;
		var chk1000 = false;

		document.location = url;
		setTimeout(function() {
			if (document.hasFocus()) {
				chk500 = true;
			}
		}, 500);
		setTimeout(function() {
			if (document.hasFocus()) {
				chk1000 = true;
			}

			if (chk500 && chk1000) {
				SendMessage(gameObjectName, methodName, 'false');
			}
			else {
				SendMessage(gameObjectName, methodName, 'true');
			}
		}, 1000);
	},

	psSetTextUI : function(isOn) {
		var instance = document.getElementById('unity-container');
		if (isOn) {
			// window.addEventListener('resize', setMinWidth)
			instance.style.minWidth = '1800px';
		}
		else {
			// window.removeEventListener('resize', setMinWidth)
			instance.style.minWidth = '1100px';
		}
	},

	psCopyToClipboard : function(text) {
		var toCopy = UTF8ToString(text);

		if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(toCopy).then(function () {
                console.debug('Copied to clipboard navigator: ' + toCopy);
            }, function (error) {
                console.error('Failed to copy to clipboard navigator', error);
            });
        } else {
            const textArea = document.createElement('textarea');
            textArea.value = toCopy;

            // Avoid scrolling to bottom
            textArea.style.top = '0';
            textArea.style.left = '0';
            textArea.style.position = 'fixed';

            document.body.appendChild(textArea);
            textArea.focus();
            textArea.select();

            try {
                const successful = document.execCommand('copy');
                const msg = successful ? 'successful' : 'unsuccessful';
                console.debug('Fallback: Copying text command was ', msg);
            } catch (err) {
                console.error('Fallback: Unable to copy', err);
            }

            document.body.removeChild(textArea);
        }
	},

	psOpenVoiceCall : function(gameObjectNamePtr, urlPtr, receivedMethodNamePtr) {
		var gameObjectName = UTF8ToString(gameObjectNamePtr);
		var url = UTF8ToString(urlPtr);
		var receivedMethodName = UTF8ToString(receivedMethodNamePtr);

		var pop = window.open(url, '', 'width=500,height=545,left=400,top=400,alwaysReised=yes');
		window.funcStopRinging = function() {
            SendMessage(gameObjectName, receivedMethodName);
        }

		if (pop) pop.focus();
	},

	psOpenPayment : function(gameObjectNamePtr, urlPtr, doneMethodNamePtr) {
		var gameObjectName = UTF8ToString(gameObjectNamePtr);
		var url = UTF8ToString(urlPtr);
		var doneMethodName = UTF8ToString(doneMethodNamePtr);

		var pop = window.open(url, 'payment', 'width=460,height=580,left=400,top=400,alwaysReised=yes');
		window.PaymentDone = function() {
            SendMessage(gameObjectName, doneMethodName);
        }

		if (pop) pop.focus();
	},

	psOpenAuth : function(gameObjectNamePtr, urlPtr, callbackMethodNamePtr) {
		var gameObjectName = UTF8ToString(gameObjectNamePtr);
		var url = UTF8ToString(urlPtr);
		var callbackMethodName = UTF8ToString(callbackMethodNamePtr);

		var pop = window.open(url, '', 'width=1280,height=960,left=400,top=400,alwaysReised=yes');
		window.funcRefreshing = function() {
            SendMessage(gameObjectName, callbackMethodName);
		}

		if (pop) pop.focus();
	},

	psOpenChat : function(urlPtr, targetSeq) {
		var url = UTF8ToString(urlPtr);

		if (! this.chat) { 
            this.chat = window.open(url, 'chat', 'width=650,height=700,left=400,top=400,alwaysReised=yes');
            if (targetSeq != 0) {
                this.chat.postMessage({ mseq: targetSeq }, url);
            }
        }
        else {
            if (this.chat.closed) {
                this.chat = window.open(url, 'chat', 'width=650,height=700,left=400,top=400,alwaysReised=yes');
                if (targetSeq != 0) {
                    this.chat.postMessage({ mseq: targetSeq }, url);
                }
            }
            else {
                this.chat.postMessage({ mseq: targetSeq }, url);
            }
        }

        this.chat.focus();
	},

	psCheckChat : function() {
		if (! this.chat)
			return false;
		else 
			return !this.chat.closed;
	},

	psConnectInnerWebview : function(gameObjectNamePtr, closeMethodNamePtr) {
		var gameObjectName = UTF8ToString(gameObjectNamePtr);
		var closeMethodName = UTF8ToString(closeMethodNamePtr);

		var tempInput = document.getElementById('unity-webview-msg');
        if (tempInput) {
            document.body.removeChild(tempInput);
        }

        var tempInput = document.createElement('input');
        tempInput.setAttribute('id', 'unity-webview-msg');
        tempInput.setAttribute('type', 'hidden');
        tempInput.onchange = function() {
            SendMessage(gameObjectName, closeMethodName);
        }
        document.body.appendChild(tempInput);
	},

	psGetLocation : function(gameObjectNamePtr, callbackMethodNamePtr) {
		var gameObjectName = UTF8ToString(gameObjectNamePtr);
		var callbackMethodName = UTF8ToString(callbackMethodNamePtr);

		navigator.geolocation.getCurrentPosition(
			function(position) {
				var data = position.coords.latitude +'|'+ position.coords.longitude;
				SendMessage(gameObjectName, callbackMethodName, data);
			},
			function(error) {
				SendMessage(gameObjectName, callbackMethodName, error.toString());
			}
		);
	},

	psSearchAddress : function(gameObjectNamePtr, callbackMethodNamePtr) {
		var gameObjectName = UTF8ToString(gameObjectNamePtr);
		var callbackMethodName = UTF8ToString(callbackMethodNamePtr);

		new daum.Postcode({
			oncomplete: function(data) {

				var addr = data.address;
				var lat = 0.0;
				var lng = 0.0;
				var result = '';

				var geocoder = new kakao.maps.services.Geocoder();
				geocoder.addressSearch(data.address, function(result, status) {
					if (status === kakao.maps.services.Status.OK) {
						lng = result[0].x
						lat = result[0].y
						result = addr +'|'+ lat +'|'+ lng;
						SendMessage(gameObjectName, methodName, result);
					}
					else {
						SendMessage(gameObjectName, methodName, addr);
					}
				});
			}
		}).open();
	},
};

mergeInto(LibraryManager.library, JsLib);