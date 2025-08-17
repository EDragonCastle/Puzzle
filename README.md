이 Branch는 Spine Animation을 제작하고 멀티 스레드를 사용하기 위해 Job System을 사용해서 동작하는 Branch입니다.
Spine Animation을 제작한 Script와 기본 로직를 멀티 스레드로 관리하고 싶어서 Untiy Job System을 이용해서 제작했습니다.
관련 코드를 보려면 해당 폴더로 가주세요.

Spine Animation 코드 구현 보기
- Empty/Assets/Script/SpineAnimation/ 
  이 폴더에 있는 cs 파일 2개 확인하기 (CloudPot, SwingGirl)

Job System 사용한 코드 보기
Job 생성
- Empty/Assets/Script/Core/BoardJob.cs
  Match Fail, Match Success Job 생성

Job 동작
- Empty/Assets/Script/Core/Board.cs
  - Board.cs 278줄의 FailSwap 함수에서 Match Fail Job을 사용한 방법에 대해 볼 수 있습니다. 
  - Board.cs 526줄의 DestoryElement 함수에서 Match Success JoB을 사용한 방법에 대해 볼 수 있습니다.

