using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PhotoFrame
{
    public partial class frmSettings :Form
    {
        //
        // メンバ変数
        //
        public Settings settings;

        //
        // コンストラクタ
        //
        public frmSettings()
        {
            // デザイナー サポートに必要なメソッド(自動生成)
            InitializeComponent();
        }

        //
        // フォーム表示
        //
        private void frmSettings_Shown( object sender, EventArgs e )
        {
            this.Text = "設定";

            txtPictureDirectoryPath.Text = settings.PictureDirectoryPath;
            txtFrameSize.Text = settings.FrameSize.ToString();
            lblFrameColor.BackColor = ColorTranslator.FromWin32( settings.FrameColorWin32 );
        }

        //
        // 画像フォルダパス ドラッグドロップ
        //
        private void txtPictureDirectoryPath_DragEnter( object sender, DragEventArgs e )
        {
            if ( e.Data.GetDataPresent( DataFormats.FileDrop ) ) {  // ファイルの場合のみ許可
                e.Effect = DragDropEffects.All;
            } else {
                e.Effect = DragDropEffects.None;
            }
        }
        //
        // 画像フォルダパス ドラッグドロップ
        //
        private void txtPictureDirectoryPath_DragDrop( object sender, DragEventArgs e )
        {
            try {
                if ( e.Data.GetDataPresent( DataFormats.FileDrop ) ) {  // ドラッグドロップで画像フォルダ指定
                    string[] dragFiles = (string[])e.Data.GetData( DataFormats.FileDrop, false );

                    // 最初の1つ目がディレクトリの場合のみ適用
                    if ( File.GetAttributes( dragFiles[0] ) == FileAttributes.Directory ) {
                        txtPictureDirectoryPath.Text = dragFiles[0];
                    }
                }
            }
            catch ( Exception ex ) {
                Debug.WriteLine( GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + ":" + ex.Message );
            }
        }

        //
        // 参照ボタンをクリック
        //
        private void btnBrowse_Click( object sender, EventArgs e )
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "画像のフォルダを選択";
            fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
            fbd.SelectedPath = settings.PictureDirectoryPath;

            if ( fbd.ShowDialog() == DialogResult.OK ) {
                txtPictureDirectoryPath.Text = fbd.SelectedPath;
            }

            fbd.Dispose();
        }

        //
        // 色選択ボタンをクリック
        //
        private void btnColorSelect_Click( object sender, EventArgs e )
        {
            ColorDialog cd = new ColorDialog();

            // 作成した色、に追加
            cd.CustomColors = new int[] {
                settings.FrameColorWin32,                       // 指定した色
                ColorTranslator.ToWin32( Color.WhiteSmoke ),    // デフォルト色

            };

            if ( cd.ShowDialog() == DialogResult.OK ) {
                lblFrameColor.BackColor = cd.Color;             // 反映
            }
        }

        //
        // OKボタンをクリック
        //
        private void btnOk_Click( object sender, EventArgs e )
        {
            if ( Directory.Exists( txtPictureDirectoryPath.Text ) ) {
                settings.PictureDirectoryPath = txtPictureDirectoryPath.Text;
            }

            uint ui;
            if ( uint.TryParse( txtFrameSize.Text, out ui ) ) {
                settings.FrameSize = (int)ui;
            }

            settings.FrameColorWin32 = ColorTranslator.ToWin32( lblFrameColor.BackColor );

            this.Close();
        }

        //
        // キャンセルボタンをクリック
        //
        private void btnCancel_Click( object sender, EventArgs e )
        {
            this.Close();
        }
    }
}
