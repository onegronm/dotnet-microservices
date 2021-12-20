### Docker
```powershell
docker build negronoa/commandservice:latest .
docker push negronoa/commandservice:latest
```

### Kubernetes
```powershell
kubectl get services
kubectl get pods
kubectl get deployments
kubectl apply -f commands-deployment.yaml
kubectl apply -f platforms-deployment.yaml
kubectl rollout restart deployment platforms-deployment
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.1.0/deploy/static/provider/cloud/deploy.yaml
kubectl get pod --namespace=ingress-nginx
kubectl get services --namespace=ingress-nginx
```
