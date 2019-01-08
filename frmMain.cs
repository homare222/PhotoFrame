using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace PhotoFrame
{
    //
    // メインフォーム
    //
    public partial class frmMain :Form
    {
        //
        // メンバ変数
        //
        private Settings        mSettings       = new Settings();
        private frmSettings     mFrmSettings    = new frmSettings();

        private List<string>    mPictureList    = new List<string>();
        private int             mPictIndex      = 0;
        private Point           mMousePoint;

        //
        // コンストラクタ
        //
        public frmMain()
        {
            // デザイナー サポートに必要なメソッド(自動生成)
            InitializeComponent();

            // ディスプレイ設定変更のイベントハンドラ
            SystemEvents.DisplaySettingsChanged += new EventHandler( SystemEvents_DisplaySettingChanged );
        }

        //
        // アプリの切り替え(Alt+Tab)に表示しない処理
        //
        const int WS_EX_TOOLWINDOW = 0x00000080;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        //
        // ディスプレイ設定変更のイベント
        //
        private void SystemEvents_DisplaySettingChanged( object sender, EventArgs e )
        {
            restoreFormBounds( this.Bounds );
        }

        //
        // フォームロード時
        //
        private void frmMain_Load( object sender, EventArgs e )
        {
            // カレントディレクトリをexeのフォルダに変更
            Directory.SetCurrentDirectory( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) );

            // フォームサイズの初期値は、画面の20%
            this.Width = (int)(Screen.GetBounds( this ).Width * 0.20);
            this.Height = this.Width;

            // 設定を読込
            Settings.Load( ref mSettings );

            // フォーム位置を復元
            if ( mSettings.FormBounds != Rectangle.Empty ) {
                restoreFormBounds( mSettings.FormBounds );
            }

            // 常に前面に表示
            this.TopMost = mSettings.TopMost;
            ToolStripMenuItemTopMost.Checked = mSettings.TopMost;
        }

        //
        // フォームの位置とサイズを復元
        //
        private void restoreFormBounds( Rectangle FormBounds )
        {
            foreach ( Screen sc in Screen.AllScreens ) {
                if ( sc.WorkingArea.Contains( FormBounds ) ) {  // どれかの画面に収まっている場合は、位置を復元
                    this.Bounds = FormBounds;
                    return;
                }
            }

            // どの画面にも収まっていない場合は、プライマリ画面の右下に表示
            this.Location = new Point( (Screen.PrimaryScreen.WorkingArea.Width - this.Width), (Screen.PrimaryScreen.WorkingArea.Height - this.Height) );
        }

        //
        // フォーム表示時
        //
        private void frmMain_Shown( object sender, EventArgs e )
        {
            // タイトル
            this.Text = Application.ProductName;
            this.notifyIcon.Text = Application.ProductName;

            // コンテキストメニューを登録
            this.ContextMenuStrip = this.contextMenuStrip;

            // タスクバーに表示しない
            this.ShowInTaskbar = false;

            // フォーム部分を透過にする
            this.TransparencyKey = SystemColors.Control;

            // 枠の設定
            setPictureFrame();

            // 画像リストを生成
            makePictureList();

            // 画像を表示
            showPicture();
        }

        //
        // フォーム閉じる
        //
        private void frmMain_FormClosed( object sender, FormClosedEventArgs e )
        {
            // フォームの情報を保存
            mSettings.FormBounds = this.Bounds;

            // 設定を保存
            Settings.Save( ref mSettings );
        }

        //
        // フォーム移動時
        //
        private void frmMain_Move( object sender, EventArgs e )
        {
            // 画面端に吸着する処理
            foreach ( Screen sc in Screen.AllScreens ) {
                if ( sc.WorkingArea.Contains( this.Bounds ) ) {
                    Rectangle   wa  = sc.WorkingArea;

                    // 吸着エリアのサイズ
                    int w = (int)(wa.Width * 0.01);
                    int h = (int)(wa.Height * 0.01);

                    // 吸着エリアは、一旦画面左上でRectangleを作成し、
                    Rectangle LeftTop       = new Rectangle( wa.X, wa.Y, w, h );
                    Rectangle LeftBottom    = new Rectangle( wa.X, wa.Y, w, h );
                    Rectangle RightTop      = new Rectangle( wa.X, wa.Y, w, h );
                    Rectangle RightBottom   = new Rectangle( wa.X, wa.Y, w, h );

                    // 位置をオフセットして四隅に
                    LeftTop.Offset( 0, 0 );
                    LeftBottom.Offset( 0, (wa.Height - h) );
                    RightTop.Offset( (wa.Width - w), 0 );
                    RightBottom.Offset( (wa.Width - w), (wa.Height - h) );

                    // 吸着の判定
                    if ( LeftTop.IntersectsWith( this.Bounds ) ) {      // 左上に吸着
                        this.Location = new Point( wa.X, wa.Y );
                        return;
                    }
                    if ( LeftBottom.IntersectsWith( this.Bounds ) ) {   // 左下に吸着
                        this.Location = new Point( wa.X, wa.Y + (wa.Height - this.Height) );
                        return;
                    }
                    if ( RightTop.IntersectsWith( this.Bounds ) ) {     // 右上に吸着
                        this.Location = new Point( wa.X + (wa.Width - this.Width), wa.Y );
                        return;
                    }
                    if ( RightBottom.IntersectsWith( this.Bounds ) ) {  // 右下に吸着
                        this.Location = new Point( wa.X + (wa.Width - this.Width), wa.Y + (wa.Height - this.Height) );
                        return;
                    }
                }
            }
        }

        //
        // 枠の設定
        //
        private void setPictureFrame()
        {
            if ( mSettings.FrameSize != 0 ) {
                this.Padding = new Padding( mSettings.FrameSize );  // フォームにパディングを設定し、疑似的に枠に見せる
                this.BackColor = ColorTranslator.FromWin32( mSettings.FrameColorWin32 );
            }
        }

        //
        // 画像リストを生成
        //
        private void makePictureList()
        {
            try {
                if ( !Directory.Exists( mSettings.PictureDirectoryPath ) ) {
                    return;
                }

                string[] jpgList = Directory.GetFiles( mSettings.PictureDirectoryPath, "*.jpg" );
                foreach ( string filePath in jpgList ) {
                    mPictureList.Add( filePath );
                }

                string[] pngList = Directory.GetFiles( mSettings.PictureDirectoryPath, "*.png" );
                foreach ( string filePath in pngList ) {
                    mPictureList.Add( filePath );
                }

                mPictIndex = mPictureList.IndexOf( mSettings.LastShowPictureFilePath );
                if ( mPictIndex < 0 ) {
                    mPictIndex = 0;
                }
            }
            catch ( Exception ex ) {
                Debug.WriteLine( GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + ":" + ex.Message );
            }
        }

        //
        // キーイベント
        //
        private void frmMain_PreviewKeyDown( object sender, PreviewKeyDownEventArgs e )
        {
            switch ( e.KeyCode ) {
            case Keys.Up:           // カーソルキー上で、5%ずつ大きく
                this.SetBounds( 0, 0, (int)(this.Width * 1.05), (int)(this.Height * 1.05), BoundsSpecified.Size );
                pctBox.Refresh();
                break;

            case Keys.Down:         // カーソルキー下で、5%ずつ小さく
                this.SetBounds( 0, 0, (int)(this.Width * 0.95), (int)(this.Height * 0.95), BoundsSpecified.Size );
                pctBox.Refresh();
                break;

            case Keys.Left:         // カーソルキー左で、前の画像
                showPrevPicture();
                break;

            case Keys.Right:        // カーソルキー右で、次の画像
                showNextPicture();
                break;

            case Keys.Oemcomma:     // Ctrl+カンマで、設定画面を表示
                if ( e.Control ) {
                    ToolStripMenuItemSettings_Click( null, null );
                }
                break;

            case Keys.Q:
                if ( e.Control ) {  // Ctrl+Qで、終了
                    this.Close();
                }
                break;

            default:
                break;
            }
        }

        //
        // コンテキストメニューの次の画像をクリック
        //
        private void ToolStripMenuItemNextPicture_Click( object sender, EventArgs e )
        {
            showNextPicture();
        }

        //
        // コンテキストメニューの前の画像をクリック
        //
        private void ToolStripMenuItemLastPicture_Click( object sender, EventArgs e )
        {
            showPrevPicture();
        }

        //
        // コンテキストメニューのランダムをクリック
        //
        private void ToolStripMenuItemRandomPicture_Click( object sender, EventArgs e )
        {
            showRandomPicture();
        }

        //
        // コンテキストメニューの最大化をクリック
        //
        private void ToolStripMenuItemChangeFormSize_Click( object sender, EventArgs e )
        {
            // 最大化 と 元のサイズに戻す をトグルする
            if ( ToolStripMenuItemChangeFormSize.Text.Contains( "最大化" ) ) {
                this.WindowState = FormWindowState.Maximized;
                ToolStripMenuItemChangeFormSize.Text = "元のサイズに戻す";
            } else {
                this.WindowState = FormWindowState.Normal;
                ToolStripMenuItemChangeFormSize.Text = "最大化";
            }
            showPicture();
        }

        //
        // コンテキストメニューの常に前面に表示をクリック
        //
        private void ToolStripMenuItemTopMost_CheckedChanged( object sender, EventArgs e )
        {
            mSettings.TopMost = ((ToolStripMenuItem)sender).Checked;
            this.TopMost = mSettings.TopMost;
        }

        //
        // コンテキストメニューの設定をクリック
        //
        private void ToolStripMenuItemSettings_Click( object sender, EventArgs e )
        {
            mFrmSettings.settings = mSettings;

            mFrmSettings.ShowDialog();

            mSettings = mFrmSettings.settings;

            setPictureFrame();
            makePictureList();
            showPicture();
        }

        //
        // コンテキストメニューの終了をクリック
        //
        private void ToolStripMenuItemExit_Click( object sender, EventArgs e )
        {
            this.Close();
        }

        //
        // ピクチャーボックス上でマウスをクリック
        //
        private void pctBox_MouseDown( object sender, MouseEventArgs e )
        {
            if ( (e.Button & MouseButtons.Left) == MouseButtons.Left ) {    // クリック時の位置を記録
                mMousePoint = new Point( e.X, e.Y );
            }
        }

        //
        // ピクチャーボックス上でマウスを移動
        //
        private void pctBox_MouseMove( object sender, MouseEventArgs e )
        {
            if ( (e.Button & MouseButtons.Left) == MouseButtons.Left ) {    // フォームをドラッグ時に、フォーム自体を移動する
                this.Left += e.X - mMousePoint.X;
                this.Top += e.Y - mMousePoint.Y;
            }
        }

        //
        // タスクトレイのアイコンをクリック
        //
        private void notifyIcon_MouseUp( object sender, MouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Left ) {  // 左クリックでアクティブに
                this.Activate();
            }
        }

        //
        // 画像を表示
        //
        private void showPicture()
        {
            try {
                FileStream  fs;
                Image       img;

                if ( mPictureList.Count == 0 ) {
                    return;
                }

                // 画像ファイルを読込
                fs = new FileStream( mPictureList[mPictIndex], FileMode.Open, FileAccess.Read );
                img = Image.FromStream( fs );

                rotateByExifOrientation( ref img ); // Exif情報による画像の回転

                // フォームの高さを調整
                double zoomLevel = (double)(this.Width - (this.Padding.Left + this.Padding.Right)) / img.Width;
                this.Height = (int)(img.Height * zoomLevel) + (this.Padding.Top + this.Padding.Bottom);

                // 画像を表示
                if ( pctBox.Image != null ) {   // 前の画像のリソースを解放
                    pctBox.Image.Dispose();
                }
                pctBox.Image = img;
                fs.Close();

                // 画像の情報を表示
                dispPictureInfo( ref img );

                mSettings.LastShowPictureFilePath = mPictureList[mPictIndex];   // 最後に表示した画像のパスを保存
            }
            catch ( Exception ex ) {
                Debug.WriteLine( GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + ":" + ex.Message );
            }
        }

        /// <summary>
        /// Exif情報のOrientation値を判断し画像を回転 
        /// </summary>
        private void rotateByExifOrientation( ref Image img )
        {
            //
            // デジタルスチルカメラ用画像ファイルフォーマット規格 Exif 2.3 より http://www.cipa.jp/std/documents/j/DC-008-2012_J.pdf
            //
            //     画像方向 Orientation
            //         
            //         行と列の観点から見た、画像の方向。
            //         Tag     =  274 (112.H)
            //         Type    =  SHORT
            //         Count   =  1
            //         Default =  1
            //         
            //         1 =  0 番目の行が目で見たときの画像の上(visual top)、0 番目の列が左側(visual left-hand side)となる。
            //         2 =  0 番目の行が目で見たときの画像の上、0 番目の列が右側(visual right-hand side)となる。
            //         3 =  0 番目の行が目で見たときの画像の下(visual bottom)、0 番目の列が右側となる。
            //         4 =  0 番目の行が目で見たときの画像の下、0 番目の列が左側となる。
            //         5 =  0 番目の行が目で見たときの画像の左側、0 番目の列が上となる。
            //         6 =  0 番目の行が目で見たときの画像の右側、0 番目の列が上となる。
            //         7 =  0 番目の行が目で見たときの画像の右側、0 番目の列が下となる。
            //         8 =  0 番目の行が目で見たときの画像の左側、0 番目の列が下となる。
            //
            // 以下が分かりやすい。
            //     
            //     Exif Orientation Tag (Feb 17 2002) http://sylvana.net/jpegcrop/exif_orientation.html
            //     
            //           1        2       3      4         5            6           7          8
            //         
            //         888888  888888      88  88      8888888888  88                  88  8888888888
            //         88          88      88  88      88  88      88  88          88  88      88  88
            //         8888      8888    8888  8888    88          8888888888  8888888888          88
            //         88          88      88  88
            //         88          88  888888  888888
            //
            try {
                // Orientation値を取得
                PropertyItem propItem = img.GetPropertyItem( 274 );
                if ( propItem == null ) {
                    return;
                }

                // Orientation値により画像を回転
                switch ( BitConverter.ToInt16( propItem.Value, 0 ) ) {
                case 1: // そのまま
                    break;

                case 2: // Y軸で反転
                    img.RotateFlip( RotateFlipType.RotateNoneFlipY );
                    break;

                case 3: // 180度回転
                    img.RotateFlip( RotateFlipType.Rotate180FlipNone );
                    break;

                case 4: // X軸で反転
                    img.RotateFlip( RotateFlipType.RotateNoneFlipX );
                    break;

                case 5: // 90度回転し、Y軸で反転
                    img.RotateFlip( RotateFlipType.Rotate90FlipY );
                    break;

                case 6: // 90度回転
                    img.RotateFlip( RotateFlipType.Rotate90FlipNone );
                    break;

                case 7: // 90度回転し、X軸で反転
                    img.RotateFlip( RotateFlipType.Rotate90FlipX );
                    break;

                case 8: // 270度回転
                    img.RotateFlip( RotateFlipType.Rotate270FlipNone );
                    break;

                default:
                    break;
                }
            }
            catch ( Exception ex ) {
                Debug.WriteLine( GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + ":" + ex.Message );
            }
        }

        //
        // 画像の情報を表示
        //
        private void dispPictureInfo( ref Image img )
        {
            try {
                ToolStripMenuItemInfo.Text = "";

                string str = "";

                // DateTimeOriginal値を取得
                if ( getImgPropItem( 36867, ref img, ref str ) ) {
                    DateTime dt;
                    if ( DateTime.TryParseExact( str, "yyyy:MM:dd HH:mm:ss", null, DateTimeStyles.None, out dt ) ) {
                        ToolStripMenuItemInfo.Text += "撮影日時:" + dt.ToString();
                    }
                }

                // Model値を取得
                if ( getImgPropItem( 272, ref img, ref str ) ) {
                    ToolStripMenuItemInfo.Text += Environment.NewLine;
                    ToolStripMenuItemInfo.Text += "デバイス:" + str;
                }
            }
            catch ( Exception ex ) {
                Debug.WriteLine( GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + ":" + ex.Message );
            }
        }

        //
        // 画像のプロパティ項目を取得(文字列)
        //
        private bool getImgPropItem( int propid, ref Image img, ref string str )
        {
            try {
                str = "";

                PropertyItem propItem = img.GetPropertyItem( propid );
                if ( propItem == null ) {
                    return false;
                }

                str = Encoding.ASCII.GetString( propItem.Value );
                str = str.Trim( '\0' );
                return true;
            }
            catch ( Exception ex ) {
                Debug.WriteLine( GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + ":" + ex.Message );
                return false;
            }
        }

        //
        // 次の画像を表示
        //
        private void showNextPicture()
        {
            mPictIndex++;
            if ( mPictIndex >= mPictureList.Count ) {
                mPictIndex = 0;
            }
            showPicture();
        }

        //
        // 前の画像を表示
        //
        private void showPrevPicture()
        {
            mPictIndex--;
            if ( mPictIndex < 0 ) {
                mPictIndex = mPictureList.Count - 1;
            }
            showPicture();
        }

        //
        // 画像をランダムに表示
        //
        private void showRandomPicture()
        {
            Random r = new Random();
            mPictIndex = r.Next( 0, (mPictureList.Count - 1) );
            showPicture();
        }
    }

    //
    // 設定クラス
    //
    public class Settings
    {
        public string       PictureDirectoryPath    = "";               // 画像ディレクトリのパス
        public string       LastShowPictureFilePath = "";               // 最後に表示した画像ファイルのパス
        public Rectangle    FormBounds              = Rectangle.Empty;  // フォーム位置
        public bool         TopMost                 = false;            // 常に前面に表示
        public int          FrameSize               = 0;                // 枠の幅
        public int          FrameColorWin32         = ColorTranslator.ToWin32( Color.WhiteSmoke ); // 枠の色 (Color型でシリアライズできないのでWindowsカラーで扱う)

        //
        // 設定ファイルのパス
        //
        private static string FilePath()
        {
            // exeと同じディレクトリの"(アプリケーション名)Settings.xml"
            return Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) + @"\" + Application.ProductName + "Settings.xml";
        }

        //
        // 設定を保存
        //
        public static void Load( ref Settings Settings )
        {
            if ( !File.Exists( FilePath() ) ) {
                return;
            }

            XmlSerializer   slzr    = new XmlSerializer( typeof( Settings ) );
            StreamReader    sr      = new StreamReader( FilePath(), new UTF8Encoding( false ) );
            Settings = (Settings)slzr.Deserialize( sr );
            sr.Close();
        }

        //
        // 設定を読込
        //
        public static void Save( ref Settings Settings )
        {
            XmlSerializer   slzr    = new XmlSerializer( typeof( Settings ) );
            StreamWriter    sw      = new StreamWriter( FilePath(), false, new UTF8Encoding( false ) );
            slzr.Serialize( sw, Settings );
            sw.Close();
        }
    }
}
