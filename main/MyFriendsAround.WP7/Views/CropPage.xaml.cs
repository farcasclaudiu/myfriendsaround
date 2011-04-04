using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using MyFriendsAround.WP7.Utils;
using MyFriendsAround.WP7.ViewModel;
using NetworkDetection;

namespace MyFriendsAround.WP7.Views
{
    public partial class CropPage : PhoneApplicationPage
    {
        private int imageSize = 200;

        public CropPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(CropPage_Loaded);
        }

        //private BitmapImage imageSrc;
        void CropPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!NetworkDetector.Instance.GetZuneStatus())
            {
                PhotoChooserTask task = new PhotoChooserTask();
                task.Show();
                task.Completed += new EventHandler<PhotoResult>(task_Completed);

                //TEST
                //imageSrc = new BitmapImage(new Uri("/icons/Penguins.jpg", UriKind.RelativeOrAbsolute));
                //image1.Source = imageSrc;

                SetPicture();
            }
            else
            {
                NavigateBack();
            }
        }

        private void NavigateBack()
        {
            Container.Instance.Resolve<MainViewModel>("MainViewModel").CropCancel();
        }

        void task_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage image = new BitmapImage();
                image.SetSource(e.ChosenPhoto);
                //
                image1.Width = this.Width/2;
                image1.Height = image1.Width*image.PixelHeight/image.PixelWidth;
                image1.Source = image;

                SetPicture();
            }
            else
            {
                NavigateBack();
            }
        }

        private Rectangle rect;
        void SetPicture()
        {
            rect = new Rectangle();
            rect.Opacity = .5;
            rect.Fill = new SolidColorBrush(Colors.White);
            rect.Height = imageSize;
            rect.MaxHeight = imageSize;
            rect.MaxWidth = imageSize;
            rect.Width = imageSize;
            rect.Stroke = new SolidColorBrush(Colors.Red);
            rect.StrokeThickness = 2;
            rect.Margin = new Thickness(0);
            ImageContainer.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(rect_ManipulationDelta);

            ImageContainer.Children.Add(rect);
            ImageContainer.Height = this.ActualWidth;
            ImageContainer.Width = this.ActualWidth;
        }

        int trX = 0;
        int trY = 0;
        double scale = 1;

        void rect_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {

            if (e.DeltaManipulation.Scale.X > 0 && e.DeltaManipulation.Scale.Y > 0)
            {
                scale = scale*(e.DeltaManipulation.Scale.X + e.DeltaManipulation.Scale.Y)/2;
            }

            trX += (int) ((double) e.DeltaManipulation.Translation.X /scale);
            trY += (int) ((int) e.DeltaManipulation.Translation.Y /scale);
            e.Handled = true;

            TransformGroup tg = new TransformGroup();

            ScaleTransform st = new ScaleTransform();
            st.CenterX = ImageContainer.ActualWidth / 2 - trX;
            st.CenterY = ImageContainer.ActualHeight / 2 - trY;
            st.ScaleX = scale;
            st.ScaleY = scale;
            tg.Children.Add(st);

            TranslateTransform tr = new TranslateTransform();
            tr.X = trX;
            tr.Y = trY;
            tg.Children.Add(tr);


            image1.RenderTransform = tg;

            //croppingRectangle
            GeneralTransform gt = image1.TransformToVisual(ImageContainer);
            Point p = gt.Transform(new Point( 0, 0));
            RectangleGeometry geo = new RectangleGeometry();
            geo.Rect = new Rect(-p.X / scale, -p.Y / scale, ImageContainer.ActualWidth / scale, ImageContainer.ActualHeight / scale);
            image1.Clip = geo;
        }

        public void Save()
        {
            WriteBitmap();
        }

        void WriteBitmap()
        {
            Rectangle r = (Rectangle)(from c in ImageContainer.Children where c.Opacity == .5 select c).First();
            GeneralTransform gt = r.TransformToVisual(image1);
            //
            WriteableBitmap wbm = new WriteableBitmap(imageSize, imageSize);
            wbm.Render(image1, gt.Inverse as Transform);
            wbm.Invalidate();

            using (MemoryStream stream = new MemoryStream())
            {
                wbm.SaveJpeg(stream, imageSize, imageSize, 0, 100);
                //
                stream.Seek(0, SeekOrigin.Begin);
                byte[] _imageBytes = new byte[stream.Length];
                stream.Read(_imageBytes, 0, _imageBytes.Length);
                //save
                IsolatedStorageHelper.SaveToLocalStorage("myphoto.jpg", "profiles", _imageBytes);
                //
                //BitmapImage bi = new BitmapImage();
                //stream.Seek(0, SeekOrigin.Begin);
                //bi.SetSource(stream);
                Container.Instance.Resolve<MainViewModel>("MainViewModel").MyPicture = wbm;
            }
        }

        private void Accept_Click(object sender, EventArgs e)
        {
            Save();
            NavigateBack();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            NavigateBack();
        }

    }
}