# SteamP2PInfo-Rubiconian

tremwil氏作のSteamP2PInfoに余分な機能を追加するプロジェクトです。  
Original: [tremwil/SteamP2PInfo](https://github.com/tremwil/SteamP2PInfo)  

Armored Core VI での使用を念頭に開発  
![](docs/image/gui.PNG)

## 追加した機能

全ては余分に過ぎないのだ…

- リレーサーバー使用有無表示
- 接続メソッド表示(SteamNetworking/SteamNetworkingSocket)
- 相手側Connection Quality表示
  - SteamNetworkingSocketでの接続時のみ。AC6では取得できない様子
- 接続情報統計表示
  - Ping, Connection Quality, Connection Quality Remoteについて下記を表示
    - 最小値
    - 最大値
    - 平均
    - 標準偏差
- 表示カラム設定

※現在オーバーレイ表示機能には未着手

## 使い方

オリジナルのSteamP2PInfoに準じます。

1. releasesからダウンロードしたZIPを展開し、`SteamP2PInfo.exe`を起動
1. ゲームを起動したらウィンドウのタイトルバー右側にある"Attach Game"をクリック
1. 対象のプロセスを選択  
   ※初回のみAppIdの設定が必要。SteamDB等で確認のこと
1. 必要に応じてSteamのコンソールコマンドを実行（手動）
