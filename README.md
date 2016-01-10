# Unity-WebVR-Test-Assets-new

Unity-WebVR-Test-Assetsをデスクトップおよびモバイル両方で動くように修正したもの。 

##使い方
WebGLTemplatesフォルダーを、
* Windowsの場合  
  C:\Program Files\Unity\Editor\Data\PlaybackEngines\webglsupport\BuildToolsのフォルダーに上書きします。
* Macの場合  
  Assetsフォルダーにコピーします。  

これで、Build Settings > WebGL > Player Settings > Publishing Settings > WebGL Template にWebVRというテンプレートが追加されます。  

次にPrefabフォルダーとScriptsフォルダーをプロジェクトのAssetsフォルダにコピーします。
PrefabsフォルダーにはWebVRCameraSetというPrefabがありますので、このカメラを適当に配置し、WebVRCameraSetにScriptsフォルダー内にあるStereoCamera.csをアタッチします。

あとは、WebGLでWebVRテンプレートを使ってビルドを行い、出力されたindex.htmlにアクセスすればできます。

ライセンスはフリーです。自由に使ってください。

##WebVRを有効にする
Polyfillライブラリは使用していないため、モバイルでは実質WebVR APIを実装した[ChromePublic_Cardboard](https://drive.google.com/folderview?id=0BzudLt22BqGRbW9WTHMtOWMzNjQ&usp=sharing#list)のみ対応。
ChromePublic_CardboardはインストールしただけではWebVRが有効になっていないので  
**試験運用機能リストの "Enable WebVR" (直リン chrome://flags#enable-webvr) にアクセスし "有効にする" リンクをクリックして有効にします。**  
(Galaxy系は次へ、それ以外は下に"今すぐ再起動"ボタンが表示されるのでボタンを押して再起動します。)

##Galaxy系のスマホ
Galaxy系などの機種ではChromeではデフォでWebGLが使用できないというのをよく聞きます。  
私もNote4を所有しており、同様にChromeではデフォでWebGLが使用できませんでした。  
理由は使用しているGPUがなんかブラックリストに引っかかっているらしいです。  
これもまた  
**試験運用機能リストの "ソフトウェアレンダリングリストをオーバーライド" (直リン: chrome://flags#ignore-gpu-blacklist) にアクセスして "有効にする" リンクをクリックし有効にします。**  
同様に"今すぐ再起動"ボタンを押して再起動します。
これで、WebGLが使えるようになります(たぶん)。  

##動作検証
Note4で試してみたら"十分なメモリが確保できなかった"といった感じのメッセージが頻発。
メッセージが出ずに起動できたら今度はフルスクリーン表示時に右側しか表示されず、ほぼ動作は無理という結果となりました。
nexsus5 nexsus6では一応動作しました。ただ、あのバレルディストーションと呼ばれる歪みがかかった表示がされません。(前バージョンのChrome WebVR版では歪み表示されたんですけどね)
