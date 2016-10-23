using System;

using UIKit;
using SlideMenuControllerXamarin;

namespace SlideMenuControllerExample
{
	public partial class LeftViewController : UITableViewController
	{
		protected LeftViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public LeftViewController() : base("LeftViewController", null)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var row = indexPath.Row;
			UIStoryboard storyboard = UIStoryboard.FromName("Main", null);
			UINavigationController navController = this.slideMenuController().MainViewController as UINavigationController;

			this.slideMenuController().RemoveRightGestures();

			switch (row)
			{
				case 0:
					
					ViewController controller = storyboard.InstantiateViewController("ViewController") as ViewController;
					controller.Title = "First View Controller";
					navController.SetViewControllers(new UIViewController[] { controller }, true);
					break;
				case 1:
					ViewController2 controller2 = storyboard.InstantiateViewController("ViewController2") as ViewController2;
					controller2.Title = "Second View Controller";
					navController.SetViewControllers(new UIViewController[] { controller2 }, true);
					break;
				case 2:
					ViewController3 controller3 = storyboard.InstantiateViewController("ViewController3") as ViewController3;
					controller3.Title = "Third View Controller";
					this.slideMenuController().AddRightGestures();
					navController.SetViewControllers(new UIViewController[] { controller3 }, true);
					break;

			}



			this.CloseLeft();

		}
	}
}

