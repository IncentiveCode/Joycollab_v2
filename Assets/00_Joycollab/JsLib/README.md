# PitchSolution JavaScript Library (for Unity WebGL)

### description :

- @author : 이효재
- @last update : 2023. 09. 18
- @version : 0.51
- @update history
    * v0.1 (2023. 02. 22) : Joycollab 에서 사용하던 클래스 정리 및 통합 (진행 중)
    * v0.2 (2023. 02. 28) : unity 2021.3.13f1 으로 업그레이드 후, windows 에서 build 안되는 문제 해결. (한글 주석이 원인으로 보임)
    * v0.3 (2023. 03. 17) : Graphic UI 와 Text UI 전환시 unity-container 에 min-widht 값을 추가하는 함수 추가. 추후 고도화 예정.
    * v0.4 (2023. 07. 27) : 팝업 관련 함수 6종 추가. 
    * v0.5 (2023. 08. 29) : 브라우저의 위치정보 확인 기능, 카카오 주소 검색 기능 추가. 
    * v0.51 (2023. 09. 18) : 함수 정보 수정.


### function list :

``` c#
// Cookie 읽고 쓰기
public static extern string psGetCookie(string name);
public static extern void psSetCookie(string name, string value);


// 간단한 window 함수
public static extern void psAlert(string message);
public static extern void psLog(string message);
public static extern void psRedirection(string url);
public static extern void psOpenWebview(string url, string id);


// 실행한 OS 와 Browser 환경 확인 - 확인 후 method 를 callback
public static extern void psCheckBrowser(string gameObjectName, string methodName);
public static extern void psCheckSystem(string gameObjectName, string methodName);


// Tray app 같은 OS 에 설치된 프로그램을 실행하기 위한 함수 - 프로그램 설치 유무 판단 후 method 를 callback
public static extern void psRunScheme(string gameObjectName, string url, string methodName);


// unity-container 에 임시로 style 부여
public static extern void psSetTextUI(bool isOn);


// 특정 영역의 텍스트를 clipboard 로 복사.
public static extern void psCopyToClipboard();


// 용도에 맞게 webview open.
//  - 음성 채팅 열기.
public static extern void psOpenVoiceCall(string gameObjectName, string url, string receivedMethodName);
//  - 결제 창 열기.
public static extern void psOpenPayment(string gameObjectName, string url, string doneMethodName);
//  - 구글/줌 인증 창 열기.
public static extern void psOpenAuth(string gameObjectName, string url, string callbackMethodName); 
//  - 채팅 창 열기. 또는 특정 대상과의 채팅 뷰 열기.
public static extern void psOpenChat(string url, int targetSeq=0);             
//  - 특정 채팅 창이 열려있는지 확인.
public static extern void psCheckChat();            
//  - 내부적으로 webview 를 열었을 때, 닫힘 또는 새로 고침 등의 기능을 연결하기 위한 함수.
public static extern void psConnectInnerWebview(string gameObjectName, string closeMethodName);  


// 위치, 주소 획득.
//  - 브라우저에서 위치정보 획득.
public static extern void psGetLocation(string gameObjectName, string callbackMethodName);
//  - Kakao API 를 이용한 주소 검색.
public static extern void psSearchAddress(string gameObjectName, string callbackMethodName);
```

### 추가 완료 항목들
1. kakao 주소 검색
2. Browser focus 처리 -> (WebGLWindow.OnFocusEvent, WebGLWindow.OnBlurEvent 로 해결)
3. clipboard 기능 -> (CopyToClipboard() 로 해결)
4. Popup 관련 기능 -> (psOpenVoiceCall, psOpenPayment, psOpenAuth, psOpenChat, psCheckChat, psConnectInnerWebview 로 해결)


### 추가 예정 항목들
1. Xmpp 관련 기능
2. Browser notification 처리
3. Janus 관련 기능
4. etc...