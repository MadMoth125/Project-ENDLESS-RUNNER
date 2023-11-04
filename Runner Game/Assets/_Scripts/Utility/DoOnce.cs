using System;

namespace Utilities
{
	public struct DoOnce
	{
		public DoOnce(bool startClosed = false)
		{
			_hasExecuted = startClosed;
		}
	
		private bool _hasExecuted;

		/* Example usage:
		 * doOnce.Do(() =>
		 * {
		 *	Debug.Log("This will be executed only once.");
		 * });
		 */
		
		public void Do(Action action)
		{
			if (!_hasExecuted)
			{
				action.Invoke();
				_hasExecuted = true;
			}
		}

		public void Reset()
		{
			_hasExecuted = false;
		}
	}
}