# CSDL7.MasterService

If you check the *Enable integration* option when creating the microservice, the necessary configurations are made automatically, so no manual configuration is needed. For more information, refer to the [Adding New Microservices](https://abp.io/docs/latest/solution-templates/microservice/adding-new-microservices) document.


## Docker Configuration for Prometheus

If you want to monitor the new microservice with Prometheus when you debug the solution, you should add the new microservice to the `prometheus.yml` file in the `etc/docker/prometheus` folder. You can copy the configurations from the existing microservices and modify them according to the new microservice. Below is an example of the `prometheus.yml` file for the `Product` microservice.

```diff
  - job_name: 'authserver'
    scheme: http
    metrics_path: 'metrics'
    static_configs:
    - targets: ['host.docker.internal:***']
    ...
+ - job_name: 'master'
+   scheme: http
+   metrics_path: 'metrics'
+   static_configs:
+   - targets: ['host.docker.internal:44346']
```

## Creating Helm Chart for the MasterService

If you want to deploy the new microservice to Kubernetes, you should create a Helm chart for the new microservice.

First, we need to add the new microservice to the `build-all-images.ps1` script in the `etc/helm` folder. You can copy the configurations from the existing microservices and modify them according to the new microservice.

```diff
...
  ./build-image.ps1 -ProjectPath "../../apps/auth-server/CSDL7.AuthServer/CSDL7.AuthServer.csproj" -ImageName csdl7/authserver
+ ./build-image.ps1 -ProjectPath "../../services/master/CSDL7.MasterService/CSDL7.MasterService.csproj" -ImageName csdl7/master
```

We need to add the connection string to the `values.csdl7-local.yaml` file in the `etc/helm/csdl7` folder.

```diff
global:
  ...
  connectionStrings:
    ...
+   master: "Server=[RELEASE_NAME]-sqlserver,1433; Database=CSDL7_Master; User Id=sa; Password=myPassw@rd; TrustServerCertificate=true; Connect Timeout=240;"
```

We need to create a new Helm chart for the new microservice. You can copy the configurations from the existing microservices and modify them according to the new microservice.

Master microservice `values.yaml` file. 

```yaml
image:
  repository: "csdl7/master"
  tag: "latest"
  pullPolicy: IfNotPresent
swagger:
  isEnabled: "true"
```

Master microservice `Chart.yaml` file. 

```yaml
apiVersion: v2
name: master
version: 1.0.0
appVersion: "1.0"
description: CSDL7 Master Service
```

Master microservice `master.yaml` file. 

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: "{{ .Release.Name }}-{{ .Chart.Name }}"
spec:
  selector:
    matchLabels:
      app: "{{ .Release.Name }}-{{ .Chart.Name }}"
  template:
    metadata:
      labels:
        app: "{{ .Release.Name }}-{{ .Chart.Name }}"
    spec:
      containers:
      - image: "{{ .Values.image.repository }}:{{ .Values.image.tag }}"
        imagePullPolicy: "{{ .Values.image.pullPolicy }}"
        name: "{{ .Release.Name }}-{{ .Chart.Name }}"
        ports:
        - name: "http"
          containerPort: 80
        env:
        - name: "DOTNET_ENVIRONMENT"
          value: "{{ .Values.global.dotnetEnvironment }}"
        - name: "ConnectionStrings__AdministrationService"
          value: "{{ .Values.global.connectionStrings.administration | replace "[RELEASE_NAME]" .Release.Name }}"
        - name: "ConnectionStrings__AbpBlobStoring"
          value: "{{ .Values.global.connectionStrings.blobStoring | replace "[RELEASE_NAME]" .Release.Name }}"
        - name: "ConnectionStrings__MasterService"
          value: "{{ .Values.global.connectionStrings.master | replace "[RELEASE_NAME]" .Release.Name }}"
          ...
```

Master microservice `master-service.yaml` file. 

```yaml
apiVersion: v1
kind: Service
metadata:
  labels:
    name: "{{ .Release.Name }}-{{ .Chart.Name }}"
  name: "{{ .Release.Name }}-{{ .Chart.Name }}"
spec:
  ports:
    - name: "80"
      port: 80
  selector:
    app: "{{ .Release.Name }}-{{ .Chart.Name }}"
```

You can *Refresh Sub Charts* in ABP Studio. Don't forget to add the following *Metadata* information to the sub-chart's *Metadata* keys. You can access this menu by opening the *Kubernetes* menu, right-clicking the *master* sub-chart, and selecting *Properties* -> *Metadata* tab.

- `projectPath`: **../../services/master/CSDL7.MasterService/CSDL7.MasterService.csproj**
- `imageName`: **csdl7/master**
- `projectType`: **dotnet**

Afterward, you should add the *Kubernetes Services* in the *Chart Properties* -> *Kubernetes Services* tab. You can add additional keys; it applies the regex patterns.

- `Key`: **.*-master$**

> This value should be the same as the [solution runner application](https://abp.io/docs/latest/studio/running-applications#properties) *Kubernetes service* value. It's necessary for browsing because when we connect to the Kubernetes cluster, we should browse the Kubernetes services instead of using the Launch URL.

We need to configure the helm chart environments for other applications.

Below is an example of the *Identity* microservice `identity.yaml` file. 

```diff
apiVersion: apps/v1
kind: Deployment
metadata:
  name: "{{ .Release.Name }}-{{ .Chart.Name }}"
spec:
  selector:
    matchLabels:
      app: "{{ .Release.Name }}-{{ .Chart.Name }}"
  template:
    metadata:
      labels:
        app: "{{ .Release.Name }}-{{ .Chart.Name }}"
    spec:
      containers:
      - image: "{{ .Values.image.repository }}:{{ .Values.image.tag }}"
        imagePullPolicy: "{{ .Values.image.pullPolicy }}"
        name: "{{ .Release.Name }}-{{ .Chart.Name }}"
        ports:
        - name: "http"
          containerPort: 80
        env:
        ...
+       - name: "OpenIddict__Resources__MasterService__RootUrl"
+         value: "http://{{ .Release.Name }}-master"
```

Below is an example of the *AuthServer* application `authserver.yaml` file. 

```diff
apiVersion: apps/v1
kind: Deployment
metadata:
  name: "{{ .Release.Name }}-{{ .Chart.Name }}"
spec:
  selector:
    matchLabels:
      app: "{{ .Release.Name }}-{{ .Chart.Name }}"
  template:
    metadata:
      labels:
        app: "{{ .Release.Name }}-{{ .Chart.Name }}"
    spec:
      containers:
      - image: "{{ .Values.image.repository }}:{{ .Values.image.tag }}"
        imagePullPolicy: "{{ .Values.image.pullPolicy }}"
        name: "{{ .Release.Name }}-{{ .Chart.Name }}"
        ports:
        - name: "http"
          containerPort: 80
        env:
        ...
        - name: "App__CorsOrigins"
-         value: "...,http://{{ .Release.Name }}-administration"
+         value: "...,http://{{ .Release.Name }}-administration,http://{{ .Release.Name }}-master"
```

Below is an example of the *WebApiGateway* application `webapigateway.yaml` file.

```diff
apiVersion: apps/v1
kind: Deployment
metadata:
  name: "{{ .Release.Name }}-{{ .Chart.Name }}"
spec:
  selector:
    matchLabels:
      app: "{{ .Release.Name }}-{{ .Chart.Name }}"
  template:
    metadata:
      labels:
        app: "{{ .Release.Name }}-{{ .Chart.Name }}"
    spec:
      containers:
      - image: "{{ .Values.image.repository }}:{{ .Values.image.tag }}"
        imagePullPolicy: "{{ .Values.image.pullPolicy }}"
        name: "{{ .Release.Name }}-{{ .Chart.Name }}"
        ports:
        - name: "http"
          containerPort: 80
        env:
        ...
+       - name: "ReverseProxy__Clusters__ServicenameWithoutSuffix__Destinations__ServicenameWithoutSuffix__Address"
+         value: "http://{{ .Release.Name }}-master"
```

## Developing the UI for the New Microservice

After adding the new microservice to the solution, you can develop the UI for the new microservice. For .NET applications, you can add the *CSDL7.Microservicename.Contracts* package to the UI application(s) to access the services provided by the new microservice. Afterwards, you can use the [generate-proxy](https://abp.io/docs/latest/cli#generate-proxy) command to generate the proxy classes for the new microservice.

```bash
abp generate-proxy -t csharp -url http://localhost:44346/ -m master --without-contracts
```

Next, start creating *Pages* and *Components* for the new microservice in the UI application(s). Similarly, if you have an Angular application, you can use the [generate-proxy](https://abp.io/docs/latest/cli#generate-proxy) command to generate the proxy classes for the new microservice and start developing the UI.

```bash
abp generate-proxy -t ng -url http://localhost:44346/ -m master
```