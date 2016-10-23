using System;

using UIKit;
using SlideMenuControllerXamarin;

namespace SlideMenuControllerExample
{
	public partial class ViewController3 : UIViewController
	{
		protected ViewController3(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public ViewController3() : base("ViewController3", null)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.AddLeftBarButtonWithImage(UIImage.FromBundle("menu"));
			this.AddRightBarButtonWithImage(UIImage.FromBundle("menu"));

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

