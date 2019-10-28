#! /bin/sh
export JAVA_HOME="/usr/lib/jvm/jdk1.7.0_07"
 
startcmd='<PRODUCT_HOME>/bin/integrator.sh start'
restartcmd='<PRODUCT_HOME>/bin/integrator.sh restart'
stopcmd='<PRODUCT_HOME>/bin/integrator.sh stop'
 
case "$1" in
start)
   echo "Starting the WSO2 Server ..."
   su -c "${startcmd}" user1
;;
restart)
   echo "Re-starting the WSO2 Server ..."
   su -c "${restartcmd}" user1
;;
stop)
   echo "Stopping the WSO2 Server ..."
   su -c "${stopcmd}" user1
;;
*)
   echo "Usage: $0 {start|stop|restart}"
exit 1
esac
