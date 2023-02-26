var _janus = null
var _pluginHandle = null
var _isConnected = false
var deviceChkCnt = 1
var opaqueId = 'audiobridgetest-' + Janus.randomString(12)
var myid = null
var audioenabled = false
var isMuted = false

// var mySeq = 0
// var roomId = 0

const initJanus = (id, server, callback) => {
	if(!_isConnected) {

		Janus.init(id, {
			debug: 'error', callback: () => { // debug', 'vdebug', /*'log', 'warn', */'error
				if (!Janus.isWebrtcSupported()) {
					callback('브라우저 설정에서 WebRTC 기능을 허용 해주십시오. (No WebRTC support...)')
					return
				}

				// Create session
				_janus = new Janus({
					server: server,
					apisecret: 'ps20200106',
					success: () => callback(null),
					// error: err => callback('회의 서버 연결 실패 하였습니다. 관리자에게 문의 하십시오.'),
					error: err => callback(err),
					destroyed: () => callback('회의 서버 연결 끊김')
				})
			}
		})
	}
}

// Device 데이터 초기화
const initDevice = (config, callback) => {
	const audioList = []
	console.log('initDevice initDevice initDevice')
	if(!_isConnected) {
		Janus.listDevices(devices => {
			if (devices.length == 0) {
				if (this.deviceChkCnt == 1) {
					this.deviceChkCnt--
					initDevice({audio: false, video: false})
				}
				return
			}

			devices.forEach(device => {
				if ('audioinput' == device.kind)
					audioList.push(device)
			})

			callback(audioList)
		}, config, callback)
	}
}

// Plugin 초기화
// const initPlugin = (seq, callback = null) => {
const initPlugin = (callback = null) => {
	// mySeq = seq

	// 미팅 룸 설정
	if(!_isConnected)
	{
		_janus.attach({
			plugin: 'janus.plugin.audiobridge',
			opaqueId: opaqueId,

			// room 플로그인 설정 성공
			success: pluginHandle => {
				_isConnected = true
				_pluginHandle = pluginHandle
				
				if(callback != null)
					callback(roomId)
				//createRoom(id, seq)
			},
			error: err => console.log( 'initplugin error : ' + err),
			consentDialog: on => {
				console.log('Consent dialog should be ' + (on ? 'on' : 'off') + ' now', 'audiobridge')
			},
			iceState: state => {
				console.log('ICE state changed to ' + state)
			},
			mediaState: (medium, on) => {
				console.log('mediaState medium = ' + medium + ', on = ' + on)
			},
			webrtcState: on => {
				console.log('Janus says our WebRTC PeerConnection is ' + (on ? 'up' : 'down') + ' now')
			},
			// 리턴 정보
			onmessage: (msg, jsep) => {
				console.log('onmessage: ', msg)

				const event = msg.audiobridge

				if (event) {
					// 가입 성공
					if (event === 'joined') {
						if(msg.id != null)
							myid = msg.id
						else
						    instance.Module.asmLibraryArg._PlayEnterRoomSound()  //다른 사람 입장시에 사운드 실행

						_pluginHandle.createOffer({
							media: {video: false},
							success: jsep => {
								_pluginHandle.send({
									message: {
										request: 'configure',
										muted: isMuted
									},
									jsep: jsep
								})
							},
							error: err => console.log("WebRTC error:", err)
						})

						// 이미 참가한 기타 사용자 설정
						if (msg.participants)
							setParticipants(msg.participants)
					} else if (event === 'roomchanged') {
						myid = msg.id

						// 이미 참가한 기타 사용자 설정
						if (msg.participants)
							setParticipants(msg.participants)
					} else if (event === 'destroyed') {
						//window.location.reload()
						// 기타 이벤트
					} else if (event === 'event') {

						if (msg.participants) {
							setParticipants(msg.participants)
							// 방 나감
						} else if (msg.leaving) {
							document.getElementById(myid).remove()
						} else if (msg.error) {
							console.log(msg.error)
						}
					}
				}

				// stur
				if (jsep) {
					_pluginHandle.handleRemoteJsep({jsep: jsep})
				}
			},
			// 자기 영상 받아오기
			onlocalstream: stream => {
			},
			onremotestream: stream => {
				Janus.attachMediaStream(document.getElementById('roomaudio'), stream)
			},
			oncleanup: () => console.log(' ::: Got a cleanup notification: we are unpublished now :::')
		})
	}
}

// 미팅 룸 생성
function createRoom(id, seq) {
	// room 생성
	// roomId = id

	console.log('createRoom : ', _pluginHandle == null)
	if(_pluginHandle == null) 
	{
		// initPlugin(mySeq, createRoom);
		// return

		setTimeout(function() {
			createRoom(id, seq);	
		}, 500);

		return;
	}

	_pluginHandle.send({
		message: {
			room: id,
			request: 'create',
			secret: 'ps20200106',
			publishers: 50 // 룸 사용자 제한 수
		},
		success: result => {
			// 방 생성 됨
			if ('created' == result.audiobridge)
				joinRoom(id, seq)
			// 이미 있음
			else if ('event' == result.audiobridge && 486 == result.error_code)
				joinRoom(id, seq)
			else
				console.log('방 생성 실패 하였습니다!')
		}
	})
}

// 룸 진입
const joinRoom = (id, seq) => {

	console.log('joinRoom : ', _pluginHandle == null)
	if(_pluginHandle == null) 
	{
		// initPlugin(mySeq, createRoom);
		// return

		setTimeout(function() {
			createRoom(id, seq);	
		}, 500);

		return;
	}



	// Join 신청
	_pluginHandle.send({
		message: {
			request: 'join',
			room: id,
			display: 'ms:' + seq,
			codec: 'opus'
		}
	})
}

// 룸 나감
const leaveRoom = (id, seq) => {
	// leave 신청
	_pluginHandle.send({
		message: {
			request: 'leave'
		}
	})
}

// 룸 삭제
const destroyRoom = (id, seq) => {
	// leave 신청
	_pluginHandle.send({
		message: {
			request: 'destroy',
			room: id,
			secret: 'ps20200106'
		}
	})
}

// Mute
const muteAudio = (id, seq) => {
	_pluginHandle.send({
		message: {
			request: 'mute',
			room: id,
			secret: 'ps20200106',
			id : myid
		}
	})
	isMuted = true
}


// Unmute
const unMuteAudio = (id, seq) => {
	_pluginHandle.send({
		message: {
			request: 'unmute',
			room: id,
			secret: 'ps20200106',
			id : myid
		}
	})
	isMuted = false
}


const setParticipants = (participants) => {
	console.log('setParticipants: ', participant)

	participants.forEach(participant => {
		const id = participant.id
		const display = participant.display
		const setup = participant.setup
		const muted = participant.muted
	})
}
