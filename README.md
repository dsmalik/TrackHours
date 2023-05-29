# TrackHours

Just a simple windows service to track the number of hours worked so far.

It runs a timer every 1 min to log active hours which stops when the user is not logged in by checking for the existence of logon screen.

It calculates assuming 8hr of work daily.
