FROM fluent/fluentd:edge-debian

USER fluent

COPY ./fluent.conf /fluentd/etc/fluent.conf

EXPOSE 24224

CMD ["fluentd", "-c", "/fluentd/etc/fluent.conf"]
