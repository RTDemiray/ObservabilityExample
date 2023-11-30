
# Observability

This project, we monitor trace data via new relic and jaeger. We also use kibana to monitor logs.




* Run:

```bash
docker-compose up
```

* Address:
```bash
http://localhost:5001 -> Order.API
http://localhost:5002 -> Stock.API
http://localhost:5003 -> Payment.API
http://localhost:5601 -> Kibana
http://localhost:16686 -> Jaeger
```
## Screenshot

![Kibana dashboard](https://github.com/RTDemiray/ObservabilityExample/blob/main/Images/Kibana.png)

![Jaeger dashboard](https://github.com/RTDemiray/ObservabilityExample/blob/main/Images/Jaeger.png)

![New Relic dashboard](https://github.com/RTDemiray/ObservabilityExample/blob/main/Images/NewRelic.png)
  
