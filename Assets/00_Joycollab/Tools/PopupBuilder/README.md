# PitchSolution PopupBuilder

### description :

- @author : 이효재
- @last update : 2023. 04. 13
- @version : 0.3
- @update history
    * v0.1 (2023. 02. 09) : TP 에서 작업했던 내용을 가지고 와서 편집.
    * v0.2 (2023. 03. 30) : 추가 정리 및 예시 기술.
    * v0.3 (2023. 04. 13) : 사용 방법 수정.

### example :

``` c#

// common
PopupController ctrl = PopupBuilder.singleton.Build();
ctrl.Title = "title text"; 
ctrl.Content = "content text";


// alert popup
ctrl.Type = ePopupType.Alert;
ctrl.AddButton(ePopupButtonType.Normal, LocalizationManager.Localize("Alert.확인"), () => {
    // do something
});
ctrl.Open();                    // 화면 중앙에 출력
ctrl.Open(100f, 100f);          // 100f, 100f 지점에 출력
ctrl.Open(100f, 100f, true);    // 100f, 100f 지점에 출력 - 3초 후 자동 삭제


// prompt poppup example
ctrl.Type = ePopupType.Prompt;
ctrl.AddButtonWithPrompt(ePopupButtonType.Normal, LocalizationManager.Localize("Alert.확인"), (value) => {
    // do something
});
ctrl.Open();                    // 화면 중앙에 출력
ctrl.Open(100f, 100f);          // 100f, 100f 지점에 출력


// confirm popup example
ctrl.Type = ePopupType.Confirm;
ctrl.AddButton(ePopupButtonType.Normal, LocalizationManager.Localize("Alert.예"), () => {
    Application.Quit();
});
ctrl.AddButton(ePopupButtonType.Warning, LocalizationManager.Localize("Alert.아니오"));
ctrl.Open();                    // 화면 중앙에 출력
ctrl.Open(100f, 100f);          // 100f, 100f 지점에 출력


// confirm popup (with option) example
ctrl.Type = ePopupType.ConfirmWithOption;
ctrl.AddButtonWithOption(ePopupButtonType.Normal, LocalizationManager.Localize("Alert.예"), (value) => {
    Debug.Log(value ? "yes, checked." : "no, unchecked.");
});
ctrl.AddButton(ePopupButtonType.Warning, LocalizationManager.Localize("Alert.아니오"), () => {
    Debug.Log("No, thanks");
});
ctrl.Open();                    // 화면 중앙에 출력
ctrl.Open(100f, 100f);          // 100f, 100f 지점에 출력


// cancelation
ctrl.RequestClose();

```