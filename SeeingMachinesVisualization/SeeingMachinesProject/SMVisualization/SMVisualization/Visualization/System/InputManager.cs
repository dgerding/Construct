using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SMVisualization.Visualization
{
	public enum GamepadInputs
	{
		ButtonA,
		ButtonB,
		ButtonX,
		ButtonY,
		DPadDown,
		DPadLeft,
		DPadRight,
		DPadUp,
		L1,
		L2,
		L3,
		LeftStickDown,
		LeftStickRight,
		LeftStickUp,
		LeftStickLeft,
		R1,
		R2,
		R3,
		RightStickDown,
		RightStickUp,
		RightStickRight,
		RightStickLeft,
		Select,
		Start
	}

	/// <summary>
	/// While registered with an InputManager, a TextInputRecord object maintains a record of all text from the start of its recording
	/// up until the end of its recording. This takes into account caps-lock, shift, etc. and maintains a value determining a recommended
	/// carot location.
	/// </summary>
	public class TextInputRecord
	{
		#region Internal Members
		internal LinkedListNode<TextInputRecord> m_Location;
		internal uint m_AuthKey;
		private int m_CarotLocation = 0;
		#endregion

		/// <summary>
		/// The authority key that the TextInputRecord object uses to reference the input.
		/// </summary>
		public uint AuthorityKey
		{
			get
			{
				return m_AuthKey;
			}
		}

		/// <summary>
		/// Recommended location of the carot within the enteredText string. Takes into account arrow keys and the Ctrl key. The carot
		/// is meant to be placed before the character that carotLocation references within the string. Not useful for multiline entries.
		/// </summary>
		public int CarotLocation
		{
			get
			{
				if (m_CarotLocation > enteredText.Length)
					m_CarotLocation = enteredText.Length;
				if (m_CarotLocation < 0)
					m_CarotLocation = 0;
				return m_CarotLocation;
			}
			set
			{
				m_CarotLocation = value;
			}
		}

		/// <summary>
		/// Constant that carotLocation can be assigned to in order to push the carot to the end of the text.
		/// </summary>
		public const int End = 10000000;

		/// <summary>
		/// Constant that carotLocation can be assigned to in order to push the carot to the beginning of the text.
		/// </summary>
		public const int Start = -1;

		/// <summary>
		/// The text entered by the user so far in this recording.
		/// </summary>
		public String enteredText = "";
	}

	/// <summary>
	/// Provides various utilities for input isolation, input tracking, and key press/release events.
	/// </summary>
	public class InputManager
	{
		#region Internal Members
		LinkedList<TextInputRecord> m_Records = new LinkedList<TextInputRecord>();
		List<uint> m_Locks = new List<uint>();

		private Keys m_HoldKey = (Keys)0;
		private long m_StartTime = -1;
		private bool m_IsHolding = false;
		private bool m_SignalRepeat = false;

		Hashtable m_KeyboardBindings = new Hashtable();

		//	We keep the state of the current and previous frames so that we can
		//		compare key states and detect presses/releases
		KeyboardState m_PrevKeyboardState, m_CurrentKeyboardState;

		MouseState m_MouseState;

		public InputManager()
		{
			m_PrevKeyboardState = Keyboard.GetState();
			m_CurrentKeyboardState = m_PrevKeyboardState;
		}
		#endregion

		/// <summary>
		/// Access input using default authority. i.e. Movement.
		/// </summary>
		public const uint DefaultAuthKey = 0;

		/// <summary>
		/// Access input using elevated authority. i.e. User Interfaces.
		/// </summary>
		public const uint ElevatedAuthKey = 1000;

		/// <summary>
		/// Access input using master authority. i.e. Lock during cutscenes.
		/// </summary>
		public const uint MasterAuthKey = (uint)0xFFFFFFFF;

		/// <summary>
		/// Amount of time that a key must be held down (in milliseconds) for a repeat pattern
		/// to start being signaled.
		/// </summary>
		public int HoldThreshold = 200;

		/// <summary>
		/// Interval at which a held key press is repeated once the repeat partern is started.
		/// </summary>
		public int RepeatInterval = 30;

		/// <summary>
		/// Updates internal input state and updates all registered TextInputRecord objects.
		/// </summary>
		public void UpdateInput()
		{
			m_PrevKeyboardState = m_CurrentKeyboardState;
			m_CurrentKeyboardState = Keyboard.GetState();

			m_MouseState = Mouse.GetState();

			//	Update all of the text input records
			bool upperCase = m_CurrentKeyboardState.IsKeyDown(Keys.LeftShift) || m_CurrentKeyboardState.IsKeyDown(Keys.RightShift) || m_CurrentKeyboardState.IsKeyDown(Keys.CapsLock);

			Keys[] pressedKeys = m_CurrentKeyboardState.GetPressedKeys();

			/* Handle key holds for key repeating */
			if (m_HoldKey != (Keys)0)
			{
				if (!m_CurrentKeyboardState.IsKeyDown(m_HoldKey))
				{
					m_StartTime = -1;
					m_IsHolding = false;
					m_HoldKey = (Keys)0;
				}
				else
				{
					//	Whether or not the threshold for starting to repeat has been broken
					if (m_IsHolding)
					{
						//	Signal repeats
						if ((DateTime.Now.Ticks - m_StartTime) / 10000 >= RepeatInterval)
						{
							m_StartTime = DateTime.Now.Ticks;
							m_SignalRepeat = true;
						}
					}
					else
					{
						//	Wait for the threshold to start signaling
						if ((DateTime.Now.Ticks - m_StartTime) / 10000 >= HoldThreshold)
						{
							m_StartTime = DateTime.Now.Ticks;
							m_IsHolding = true;
						}
					}
				}
			}

			#region TextInputRecord updates
			foreach (Keys key in pressedKeys)
			{
				//	Only deal with keys that were recently pressed
				if (!m_PrevKeyboardState.IsKeyUp(key) && !(key == m_HoldKey && m_SignalRepeat))
					continue;
				m_SignalRepeat = false;

				//SecretHistoria.Commandline.WriteLine("Pressed key " + (int)key);

				if (m_HoldKey != key)
				{
					m_HoldKey = key;
					m_StartTime = DateTime.Now.Ticks;
					m_IsHolding = false;
				}

				for (int i = 0; i < m_Records.Count; i++)
				{
					TextInputRecord record = m_Records.ElementAt(i);
					if (!PassesAuthorityCheck(record.AuthorityKey))
						continue;

					//	Now begins the process of determining what characters to add/remove based on the key pressed
					if (key == Keys.Back)
					{
						if (record.CarotLocation > 0)
						{
							record.enteredText = record.enteredText.Remove(--record.CarotLocation, 1);
						}
					}
					if (key == Keys.Delete)
					{
						if (record.CarotLocation < record.enteredText.Length)
						{
							record.enteredText = record.enteredText.Remove (record.CarotLocation, 1);
						}
					}
					if (key == Keys.End)
					{
						record.CarotLocation = record.enteredText.Length;
					}
					if (key == Keys.Left)
					{
						--record.CarotLocation;
					}
					if (key == Keys.Right)
					{
						++record.CarotLocation;
					}
					else
					{
						if (key >= Keys.A && key <= Keys.Z)
						{
							if (upperCase)
								record.enteredText = record.enteredText.Insert(record.CarotLocation++, key.ToString().ToUpper());
							else
								record.enteredText = record.enteredText.Insert(record.CarotLocation++, key.ToString().ToLower());
						}
						else
						{
							Char newChar = '\0';
							//	Miscellaneous keys which are a pain to map (<3 XNA)
							switch (key)
							{
								//	Integer-based cases found by trial-and-error
								case ((Keys)188): if (!upperCase) newChar = ','; else newChar = '<'; break;
								case ((Keys)190): if (!upperCase) newChar = '.'; else newChar = '>'; break;
								case ((Keys)191): if (!upperCase) newChar = '/'; else newChar = '?'; break;
								case ((Keys)186): if (!upperCase) newChar = ';'; else newChar = ':'; break;
								case ((Keys)222): if (!upperCase) newChar = '\''; else newChar = '"'; break;
								case ((Keys)219): if (!upperCase) newChar = '['; else newChar = '{'; break;
								case ((Keys)221): if (!upperCase) newChar = ']'; else newChar = '}'; break;
								case ((Keys)220): if (!upperCase) newChar = '\\'; else newChar = '|'; break;
								case ((Keys)48): if (!upperCase) newChar = '0'; else newChar = ')'; break;
								case ((Keys)49): if (!upperCase) newChar = '1'; else newChar = '!'; break;
								case ((Keys)50): if (!upperCase) newChar = '2'; else newChar = '@'; break;
								case ((Keys)51): if (!upperCase) newChar = '3'; else newChar = '#'; break;
								case ((Keys)52): if (!upperCase) newChar = '4'; else newChar = '$'; break;
								case ((Keys)53): if (!upperCase) newChar = '5'; else newChar = '%'; break;
								case ((Keys)54): if (!upperCase) newChar = '6'; else newChar = '^'; break;
								case ((Keys)55): if (!upperCase) newChar = '7'; else newChar = '&'; break;
								case ((Keys)56): if (!upperCase) newChar = '8'; else newChar = '*'; break;
								case ((Keys)57): if (!upperCase) newChar = '9'; else newChar = '('; break;
								case ((Keys)189): if (!upperCase) newChar = '-'; else newChar = '_'; break;
								case ((Keys)187): if (!upperCase) newChar = '='; else newChar = '+'; break;

								case (Keys.Space): newChar = ' '; break;
							}

							if (newChar != '\0')
							{
								record.enteredText = record.enteredText.Insert(record.CarotLocation++, new String(newChar, 1));
							}
						}
					}
				}
			}
			#endregion
		}

		/// <summary>
		/// Binds the input event to the given identifier for interpretation by the InputManager.
		/// </summary>
		/// <param name="binding">Name of the identifier to be bound to.</param>
		/// <param name="key">The specific input type to listen for.</param>
		public void Bind(String binding, Keys key)
		{
			List<Keys> bindings;

			if (!m_KeyboardBindings.ContainsKey(binding))
				m_KeyboardBindings.Add(binding, bindings = new List<Keys>());
			else
				bindings = m_KeyboardBindings[binding] as List<Keys>;

			bindings.Add(key);
		}

		/// <summary>
		/// Returns whether or not the inputs assigned to the given binding are currently being triggered.
		/// </summary>
		/// <param name="binding">The identifier of the bind to be checked.</param>
		/// <param name="authKey">The authority key to be used while checking for input.</param>
		/// <returns>Whether or not the binding is being signalled. If the authKey does not pass the authority check, it returns false. If the binding does not exist, it returns false.</returns>
		public bool Check(String binding, uint authKey)
		{
			if (!PassesAuthorityCheck(authKey))
				return false;

			if (!m_KeyboardBindings.ContainsKey(binding))
				return false;

			bool check = false;
			foreach (Keys key in (m_KeyboardBindings[binding] as List<Keys>))
			{
				if (m_CurrentKeyboardState.IsKeyDown(key))
				{
					check = true;
					break;
				}
			}

			return check;
		}

		/// <summary>
		/// Returns whether or not any of the inputs assigned to the given binding were recently pressed. This
		/// only returns true upon the initial press, not while the inputs are being pressed.
		/// </summary>
		/// <param name="binding">The identifier of the bind to be checked.</param>
		/// <param name="authKey">The authority key to be used while checking for input.</param>
		/// <returns>Whether or not the bind was pressed. If the authKey does not pass the authority check, it returns false. If the binding does not exist, it returns false.</returns>
		public bool CheckPressed(String binding, uint authKey)
		{
			if (!PassesAuthorityCheck(authKey))
				return false;

			if (!m_KeyboardBindings.ContainsKey(binding))
				return false;

			bool pressed = false;

			foreach (Keys key in (m_KeyboardBindings[binding] as List<Keys>))
			{
				if (m_CurrentKeyboardState.IsKeyDown(key) && m_PrevKeyboardState.IsKeyUp(key))
				{
					pressed = true;
					break;
				}
			}

			return pressed;
		}

		/// <summary>
		/// Returns whether or not any of the inputs assigned to the given binding were recently released. This only
		/// returns true upon the initial release, not while the inputs are currently released.
		/// </summary>
		/// <param name="binding">The identifier of the bind to be checked.</param>
		/// <param name="authKey">The authority key to be used while checking for input.</param>
		/// <returns>Whether or not the bind was released. If the authKey does not pass the authority check, it returns false. If the binding does not exist, it returns false.</returns>
		public bool CheckReleased(String binding, uint authKey)
		{
			if (!PassesAuthorityCheck(authKey))
				return false;

			if (!m_KeyboardBindings.ContainsKey(binding))
				return false;

			bool released = false;

			foreach (Keys key in (m_KeyboardBindings[binding] as List<Keys>))
			{
				if (m_CurrentKeyboardState.IsKeyUp(key) && m_PrevKeyboardState.IsKeyDown(key))
				{
					released = true;
					break;
				}
			}

			return released;
		}

		/// <summary>
		/// Retrieve the current keyboard state.
		/// </summary>
		/// <param name="authKey">The authority key to be used while checking for input.</param>
		/// <returns>A KeyboardState object containing the current state of the keyboard. If the authKey does not pass the authority
		/// check, it returns an empty KeyboardState object.</returns>
		public KeyboardState GetKeyboard(uint authKey)
		{
			if (PassesAuthorityCheck(authKey))
				return m_CurrentKeyboardState;
			else
				return new KeyboardState();
		}

		public MouseState GetMouse(uint authKey)
		{
			if (PassesAuthorityCheck(authKey))
				return m_MouseState;
			else
				return new MouseState();
		}

		/// <summary>
		/// Begins recording input to detect character inputs, i.e. typing a person's name into a textbox.
		/// </summary>
		/// <param name="authKey">The authority key to be used while checking for input within this recording.</param>
		/// <returns>A TextInputRecord object to be referenced while recording input. Must be freed when finished by using StopCharacterRecording().</returns>
		public TextInputRecord StartTextRecording(uint authKey)
		{
			TextInputRecord newRecord = new TextInputRecord();
			newRecord.m_AuthKey = authKey;
			newRecord.enteredText = "";
			newRecord.CarotLocation = 0;
			newRecord.m_Location = m_Records.AddLast(newRecord);
			return newRecord;
		}

		/// <summary>
		/// Begins text recording using a pre-existing TextInputRecord object, resuming recording from the previous
		/// carot location and previous text. If oldRecord is null, it returns a newly-registered TextInputRecord object.
		/// </summary>
		/// <param name="authKey">Authority key to register the TextInputRecord object with.</param>
		/// <param name="oldRecord">The record to resume recording from.</param>
		/// <returns></returns>
		public TextInputRecord StartTextRecording(uint authKey, TextInputRecord oldRecord)
		{
			if (oldRecord == null)
				return StartTextRecording(authKey);

			oldRecord.m_AuthKey = authKey;
			oldRecord.m_Location = m_Records.AddLast(oldRecord);

			return oldRecord;
		}

		/// <summary>
		/// Stops recording character inputs and clears the internal list of recorded characters.
		/// </summary>
		/// <param name="record">The record to stop recording into.</param>
		public void StopTextRecording(TextInputRecord record)
		{
			m_Records.Remove(record.m_Location);
		}

		/// <summary>
		/// If the InputManager state has no locks, this function will always return true. If there are locks, then the check return false if there are any
		/// keys that are greater than the given authority key. If the given key is greater than or equal to the key specified by each lock, then the function
		/// returns true. If there is both an elevated lock and a master lock, then only a master authority key will pass since it is the only key greater
		/// than or equal to both.
		/// </summary>
		/// <param name="authKey">The authority key to check.</param>
		/// <returns>Whether or not the key passes the authority check.</returns>
		public bool PassesAuthorityCheck(uint authKey)
		{
			if (m_Locks.Count == 0)
				return true;

			bool passes = true;
			foreach (uint authLock in m_Locks)
			{
				if (authKey < authLock)
				{
					passes = false;
					break;
				}
			}
			return passes;
		}

		/// <summary>
		/// Adds a lock using the authority of the given key.
		/// </summary>
		/// <param name="authKey">The authority key used to create the lock.</param>
		public void Lock(uint authKey)
		{
			m_Locks.Add(authKey);
		}

		/// <summary>
		/// Removes a single lock that matches the given authority key.
		/// </summary>
		/// <param name="authKey">The key to use to match and unlock.</param>
		public void Unlock(uint authKey)
		{
			if (!m_Locks.Remove(authKey))
			{
				// TODO: Handle this situation
			}
		}
	}
}
