import React, { useState, useEffect } from "react";
import { Button, Table, Typography, message } from "antd";
import { useDetectImage } from "../../api/queries";
import ImgUploader, { formatConfidence, formatElapsedMs } from "./components/ImgUploader";

const columns = [
  { title: "類別標籤", dataIndex: "label", key: "label" },
  {
    title: "信心分數",
    dataIndex: "confidence",
    key: "confidence",
    render: (val) => formatConfidence(val),
  },
  { title: "X", dataIndex: "x", key: "x" },
  { title: "Y", dataIndex: "y", key: "y" },
  { title: "寬度", dataIndex: "width", key: "width" },
  { title: "高度", dataIndex: "height", key: "height" },
];

const ImgReadPage = () => {
  const [selectedFile, setSelectedFile] = useState(null);
  const { mutate, isLoading, data, error, reset } = useDetectImage();

  useEffect(() => {
    if (!error) return;
    if (error.response) {
      message.error("辨識失敗，請稍後再試");
    } else if (error.request || error.code === "ECONNABORTED") {
      message.error("網路錯誤，請確認連線後重試");
    }
  }, [error]);

  return (
    <div>
      <Typography.Title level={4}>影像辨識</Typography.Title>
      <ImgUploader
        onFileSelect={(file) => setSelectedFile(file)}
        onFileRemove={() => {
          setSelectedFile(null);
          reset();
        }}
        disabled={isLoading}
        detections={data?.detections}
      />
      <div style={{ marginTop: 16 }}>
        <Button
          type="primary"
          disabled={!selectedFile || isLoading}
          loading={isLoading}
          onClick={() => mutate(selectedFile)}
        >
          開始辨識
        </Button>
      </div>
      {data && (
        <div style={{ marginTop: 16 }}>
          <Typography.Text>{formatElapsedMs(data.elapsedMs)}</Typography.Text>
          {data.detections.length > 0 ? (
            <div style={{ marginTop: 8 }}>
              <Typography.Text>共偵測到 {data.detections.length} 個物件</Typography.Text>
              <Table
                style={{ marginTop: 8 }}
                dataSource={data.detections}
                columns={columns}
                rowKey={(record, index) => `${record.label}-${record.x}-${record.y}-${index}`}
                pagination={false}
              />
            </div>
          ) : (
            <div style={{ marginTop: 8 }}>
              <Typography.Text>未偵測到任何物件</Typography.Text>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default ImgReadPage;
