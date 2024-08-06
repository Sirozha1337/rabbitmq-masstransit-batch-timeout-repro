Batch Consumer stops working correctly after hitting consumer timeout

Steps to reproduce (using my example project):
1. Start Consumer
2. Start Publisher
3. Observe output, it publishes 6 messages, that should be processed in a Batch after 2 minutes
4. Wait for 2 minutes and check consumer output
5. Observe that consumer was shutdown due to Timeout
6. Wait for 2 more minutes and check consumer output
7. Observe that consumer was restarted, messages were processed but their acknowledgement failed
8. Wait for 2 more minutes and see it happen again