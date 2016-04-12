using System;
using System.Threading;
using System.Collections;

namespace LoggerX
{
	// enumeration of Scheduler events used by the delegate
	public enum SchedulerEventType { CREATED, DELETED, INVOKED };

	// delegate for Scheduler events
	public delegate void SchedulerEventDelegate(SchedulerEventType type, string scheduleName);

	// This is the main class which will maintain the list of Schedules
	// and also manage them, like rescheduling, deleting schedules etc.
	public sealed class Scheduler
	{
		// Event raised when for any event inside the scheduler
		static public event SchedulerEventDelegate OnSchedulerEvent;

		// next event which needs to be kicked off,
		// this is set when a new Schedule is added or after invoking a Schedule
		static Schedule m_nextSchedule = null;
		static ArrayList m_schedulesList = new ArrayList(); // list of schedles
		static Timer m_timer = new Timer(new TimerCallback(DispatchEvents), // main timer
											null,
											Timeout.Infinite,
											Timeout.Infinite);

		// Get schedule at a particular index in the array list
		public static Schedule GetScheduleAt(int index)
		{
			if (index < 0 || index >= m_schedulesList.Count)
				return null;
			return (Schedule)m_schedulesList[index];
		}

		// Number of schedules in the list
		public static int Count()
		{
			return m_schedulesList.Count;
		}

		// Indexer to access a Schedule object by name
		public static Schedule GetSchedule(string scheduleName)
		{
			for (int index=0; index < m_schedulesList.Count; index++)
				if (((Schedule)m_schedulesList[index]).Name == scheduleName)
					return (Schedule)m_schedulesList[index];
			return null;
		}

		// call back for the timer function
		static void DispatchEvents(object obj) // obj ignored
		{
			if (m_nextSchedule == null)
				return;
			m_nextSchedule.TriggerEvents(); // make this happen on a thread to let this thread continue
			if (m_nextSchedule.Type == ScheduleType.ONETIME)
			{
				RemoveSchedule(m_nextSchedule); // remove the schedule from the list
			}
			else
			{
				if (OnSchedulerEvent != null)
					OnSchedulerEvent(SchedulerEventType.INVOKED, m_nextSchedule.Name);
				m_schedulesList.Sort();
				SetNextEventTime();
			}
		}

		// method to set the time when the timer should wake up to invoke the next schedule
		static void SetNextEventTime()
		{
			if (m_schedulesList.Count == 0)
			{
				m_timer.Change(Timeout.Infinite, Timeout.Infinite); // this will put the timer to sleep
				return;
			}
			m_nextSchedule = (Schedule)m_schedulesList[0];
			TimeSpan ts = m_nextSchedule.NextInvokeTime.Subtract(DateTime.Now);
			if (ts < TimeSpan.Zero)
					ts = TimeSpan.Zero; // cannot be negative !
			m_timer.Change((int)ts.TotalMilliseconds, Timeout.Infinite); // invoke after the timespan
		}

		// add a new schedule
		public static void AddSchedule(Schedule s)
		{
			if (GetSchedule(s.Name) != null)
				throw new SchedulerException("Schedule with the same name already exists");
			m_schedulesList.Add(s);
			m_schedulesList.Sort();
			// adjust the next event time if schedule is added at the top of the list
			if (m_schedulesList[0] == s)
				SetNextEventTime();
			if (OnSchedulerEvent != null)
				OnSchedulerEvent(SchedulerEventType.CREATED, s.Name);
		}

		// remove a schedule object from the list
		public static void RemoveSchedule(Schedule s)
		{
			m_schedulesList.Remove(s);
			SetNextEventTime();
			if (OnSchedulerEvent != null)
				OnSchedulerEvent(SchedulerEventType.DELETED, s.Name);
		}

		// remove schedule by name
		public static void RemoveSchedule(string name)
		{
			RemoveSchedule(GetSchedule(name));
		}
	}
}
