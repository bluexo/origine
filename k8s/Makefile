REGISTRY=registry.docker.com/cluster
VERSION=latest
NAMESPACE=origine

API_PROJECT_NAME=webapi
SILO_PROJECT_NAME=host
DASHBOARD_PROJECT_NAME=dashboard
SERVER_PROJECT_NAME=gameserver

MONGO_DATA_DIR=/usr/local/origine/db/mongodb
POSTGRE_DATA_DIR=/usr/local/origine/db/postgresql

deploy: deploycluster deployserver #deploydb

createsecret:
	kubectl create secret docker-registry harborsecret --docker-server $(REGISTRY) -n $(NAMESPACE)

deployserver: 
	sed \
	-e 's,gameserver-image,$(REGISTRY)/$(SERVER_PROJECT_NAME):$(VERSION),' \
	deploy.server.yaml | kubectl apply -f -

deploycluster:	
	sed \
	-e 's,orleans-silo-image,$(REGISTRY)/$(SILO_PROJECT_NAME):$(VERSION),' \
	-e 's,orleans-api-image,$(REGISTRY)/$(API_PROJECT_NAME):$(VERSION),' \
	-e 's,orleans-dashboard-image,$(REGISTRY)/$(DASHBOARD_PROJECT_NAME):$(VERSION),' \
	deploy.cluster.yaml | kubectl apply -f -

deploydb:
	mkdir $(MONGO_DATA_DIR) -p
	mkdir $(POSTGRE_DATA_DIR) -p
	sed \
	-e 's,mongo-data-dir,$(MONGO_DATA_DIR),' \
	-e 's,postgresql-data-dir,$(POSTGRE_DATA_DIR),' \
	deploy.db.yaml | kubectl apply -f -

copysql:
	cp db.sql $(POSTGRE_DATA_DIR)

cleanserver:
	kubectl delete -f deploy.server.yaml

cleancluster:
	kubectl delete -f deploy.cluster.yaml

cleandb:
	kubectl delete -f deploy.db.yaml

clean: cleancluster cleanserver

reset: clean deploy