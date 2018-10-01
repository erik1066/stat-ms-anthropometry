docker-build:
	docker build \
		-t fdns-ms-anthstat \
		--rm \
		--no-cache \
		--build-arg ASPNETCORE_ENVIRONMENT=production \
		--build-arg APP_NAME=Fdns-AnthStat \
		.

docker-run: docker-start
docker-start:
	docker-compose up -d
	docker run -d \
		-p 9093:9093 \
		--network=fdns-ms-anthstat_default  \
		--name=fdns-ms-anthstat_main \
		fdns-ms-anthstat

docker-stop:
	docker stop fdns-ms-anthstat_main || true
	docker rm fdns-ms-anthstat_main || true
	docker-compose down

docker-restart:
	make docker-stop 2>/dev/null || true
	make docker-start