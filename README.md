# Joycollab Optimization.


## 주요 변경 사항
1. Unity version upgrade (2020.3.32f1 -> 2021.3.19f1)
2. 압축 방식 변경 (Disabled -> Brotli)
3. Singleton (DontDestroyOnLoad) 형태로 사용되는 object 축소.
   static class 또는 static function 사용으로 변경.
4. jslib 최적화. 
   여기저기 흩어져 있는 jslib 를 한 곳으로 모으고, 정리.
5. sprite 최적화.
   사용하지 않는 항목들은 정리하고, 사용하는 항목들은 크기를 POT 에 맞춰 Unity 에서 압축을 할 수 있도록 변경.
6. script 최적화.
7. list 최적화.
   가능하다면 recycle view 를 적용.
   

## 작업 진행 사항
1. jslib 정리 (진행 중)
2. UIWindow 정리. (진행 중)

	모든 윈도우의 부모가 될 창, TextUI 까지 고려해야 함
		
3. (진행할 때 마다 추가 예정)
   