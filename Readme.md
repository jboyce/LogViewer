# Summary
This project serves two functions.  It receives structured log messages from external applications over http and provides a rich UI for viewing these log messages in a web page.

# Features
 - Log messages are shown in a table format of individual fields as opposed to a simple flat list of message strings.
 - The fields displayed are not fixed.  Any custom fields can be logged and are shown in a collapsable area under each log entry row.
 - Log entries can be sorted in ascending or descending order by timestamp.
 - Warning and higher log levels are color coded to highlight log entries of significance.
 - Log entries can be filtered using a search string that is matched on the message field.
 - Optional advanced searching allows C# lambda expressions to be typed in to enable powerful matching on any log entry fields, including custom fields.

# Setup
The same executable listens for log events and hosts the web page for viewing logs.  The host url can be configured in the exe.config file.
Log events are expected to be posted over http using form url encoding content.  The NLog logging framework supports this protocol out of the box and can be enabled with the following configuration.  Note the hostname and port in the url should match those in the exe.config.  Also note that each parameter element in the config represents a field to be logged and can be customized to make all your wildest dreams come true.

	<target type='WebService'
                name='ws'
                url='http://localhost:9000/log'
                protocol='HttpPost'
                encoding='UTF-8'>
      <parameter name='Message' type='System.String' layout='${message}'/>
      <parameter name='Level' type='System.String' layout='${level}'/>
      <parameter name='Timestamp' type='System.DateTime' layout='${date}'/>
    </target>