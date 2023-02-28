# PitchSolution JavaScript Library
for Unity WebGL 


description :
@author : 이효재
@last update : 2023. 02. 28
@version : 1.1
@update history
    - [2023. 02. 22] v1.0 - Joycollab 에서 사용하던 클래스 정리 및 통합 (진행 중)
    - [2023. 02. 28] v1.1 - unity 2021.3.13f1 으로 업그레이드 후, windows 에서 build 안되는 문제 해결. (한글 주석이 원인으로 보임)


function list :

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
```

계속 추가 예정.