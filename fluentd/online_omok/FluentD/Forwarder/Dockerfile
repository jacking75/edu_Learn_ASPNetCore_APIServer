FROM fluent/fluentd:edge-debian

USER root

COPY ./fluent.conf /fluentd/etc/fluent.conf

EXPOSE 24224

CMD ["fluentd", "-c", "/fluentd/etc/fluent.conf"]
