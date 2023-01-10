# 산소덜포함 (2021)

### 프로젝트 소개
- 산소덜포함은 원작 'Oxygen Not Included'를 모작한 게임입니다.
- 총 2명의 팀원이 참여하였고, 개발 기간은 2주입니다.


### 게임 소개
- 산소덜포함은 미지의 소행성에서 살아남으며 탈출하면 승리하는 게임입니다.
- 산소가 제한적으로 존재하며, 산소가 부족하면 캐릭터들이 사망하여 미션에 실패합니다.
- 캐릭터들은 낮에는 일을, 밤에는 잠을 잡니다.
- '금' 자원을 일정량 획득하여 우주선을 만들어 탈출하는 것이 게임의 목표입니다.


### 개발 스킬
- A* 알고리즘을 기반으로 우리 게임에 맞는 길찾기 알고리즘을 개발하였습니다.
- BSP와 셀룰러오토마타 알고리즘을 응용하여 매 플레이마다 랜덤으로 만들어지는 맵을 제작하였습니다.
- 자원을 채굴함에 따라 변경되는 맵 모양에 맞추어 자동으로 8방향 사각 타일이 변경되는 알고리즘을 개발하였습니다.
- 산소가 많은 곳에서 적은 곳으로 이동하여 산소가 퍼지는 알고리즘을 개발하였습니다.
- 다수의 그림 리소스를 직접 그려 제작하였습니다.


### 게임 플레이
![1](https://user-images.githubusercontent.com/70570420/183586761-2220b2eb-760e-4f83-b04a-66ab68b9ede4.PNG)
![2](https://user-images.githubusercontent.com/70570420/183586784-569239cf-a2b8-4b3e-9eb1-b30c885ffb6e.PNG)
![4](https://user-images.githubusercontent.com/70570420/183586800-f3023abf-cba5-42e0-af93-91ac488a0d33.png)
![5](https://user-images.githubusercontent.com/70570420/183586812-0f564d74-f7ce-4fe7-82af-5ecbaa745064.PNG)
[플레이 영상](https://youtu.be/zhxk8hkDZ20)


### 내 역할과 업무성과
 - 랜덤맵 생성 알고리즘 개발
 > 이진 공간 분할법(Binary Space Partitioning)을 이용해 구역을 나눈 후, 각 구역마다 셀룰러 오토마타 방식을 응용하여 랜덤한 동굴모양의 공간(산소)들을 만드는 방식으로 매 게임마다 랜덤한 형태의 맵을 생성하도록 구현하였습니다.
![랜덤맵](https://user-images.githubusercontent.com/70570420/194479565-72d3ccac-2c69-4a0e-a363-94e6ee0aff1b.png)
[소스코드](https://github.com/JSeongJini/OxygenLessIncluded/blob/main/Assets/Scripts/Maps/RandomMapBuilder.cs)

- 맵의 변화에 따라 타일의 모양이 자동으로 변화되는 알고리즘 개발
> 돌(사암)을 채굴할 때 마다 콜백을 이용해 그 주변의 돌들이 8방향 타일에 맞추어 변경되도록 구현하였습니다.
![타일맵3](https://user-images.githubusercontent.com/70570420/194478827-e82bda2e-1bba-463a-a0d0-abf6b6573b31.png)

- 산소의 흐름 알고리즘 개발
> 산소가 많은 곳에서 적은 곳으로 이동하도록 하여, 공간 내 산소의 분포가 고르게 되도록 구현하였습니다.
![산소20221007_183626](https://user-images.githubusercontent.com/70570420/194523466-19c0a789-9985-4804-9080-9df7041a914d.gif)
```C#
//근처에 산소가 있다면
if (neighbourAir)
{
    float ownAmount = resourceMap[_pos.x, _pos.y].GetAmount();
    float negihbourAmount = neighbourAir.GetAmount();

    while (Mathf.Abs(negihbourAmount - ownAmount) > 20f)
    {
        //산소량이 많은 산소에서 적은 산소로 산소 이동
        if (negihbourAmount >= ownAmount)
        {
            float delta = Mathf.Lerp(0f, (negihbourAmount - ownAmount), 0.1f);
            resourceMap[_pos.x, _pos.y].Gain(delta);
            neighbourAir.Consume(delta);
        }
        else
        {
            float delta = Mathf.Lerp(0f, (ownAmount - negihbourAmount), 0.1f);
            resourceMap[_pos.x, _pos.y].Consume(delta);
            neighbourAir.Gain(delta);
        }

        yield return new WaitForSeconds(1f);

        ownAmount = resourceMap[_pos.x, _pos.y].GetAmount();
        negihbourAmount = neighbourAir.GetAmount();
    }
}
````
