### Docker
```powershell
docker build -t negronoa/platformservice:latest .
docker push negronoa/platformservice:latest
docker build -t negronoa/commandservice:latest .
docker push negronoa/commandservice:latest
```

### Kubernetes
```powershell
kubectl get services
kubectl get deployments
kubectl get pods
kubectl apply -f commands-deployment.yaml
kubectl apply -f platforms-deployment.yaml
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.1.0/deploy/static/provider/cloud/deploy.yaml
kubectl get pod --namespace=ingress-nginx
kubectl get services --namespace=ingress-nginx
kubectl create secret generic mssql --from-literal=SA_PASSWORD="pa550rd!"
kubectl delete deployment platforms-deployment
kubectl rollout restart deployment platforms-deployment
kubectl apply -f rabbitmq-deployment.yaml
```

### Notes
- Rather than using Docker Compose, the environments are defined through Kubernetes manifest files