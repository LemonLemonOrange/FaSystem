import React, { useMemo, useState } from 'react';
import { Button, Card, Form, Input, Select, Space, Table, Tag, Typography, message } from 'antd';
import { useDroughtAlert } from 'libs/Ncdr';
import { useReservoirRealTimeInfo, useReservoirStation } from 'libs/WraGov/reservoir';

const DEFAULT_RESERVOIR_OPTIONS = [
    '寶山水庫',
    '寶山第二水庫',
    '永和山水庫',
    '石門水庫',
    '翡翠水庫',
    '鯉魚潭水庫',
    '德基水庫',
    '南化水庫',
    '蘭潭水庫',
    '仁義潭水庫',
    '曾文水庫',
    '烏山頭水庫',
];

const CITY_CODE_TO_NAME = {
    '63': '臺北',
    '66': '臺中',
    '67': '臺南',
    '68': '桃園',
    '10002': '宜蘭',
    '10004': '新竹',
    '10005': '苗栗',
    '10007': '彰化',
    '10008': '南投',
    '10009': '雲林',
    '10010': '嘉義',
    '10013': '屏東',
    '10014': '臺東',
    '10015': '花蓮',
    '10016': '澎湖',
    '10017': '基隆',
    '10018': '新竹',
    '10020': '嘉義',
};

const WARNING_SEVERITY_META = {
    Minor: { text: '水情提醒', color: 'green' },
    Moderate: { text: '減壓供水', color: 'gold' },
    Severe: { text: '減量供水', color: 'orange' },
    Extreme: { text: '分區供水或定點供水', color: 'red' },
};

const normalizeRegion = (regionName) => String(regionName || '')
    .replace(/臺/g, '台')
    .replace(/(市|縣)$/g, '')
    .trim();

const INITIAL_MAPPINGS = [
    {
        reservoirName: '石門水庫',
        factoryName: 'HS1,3',
    },
    {
        reservoirName: '鯉魚潭水庫',
        factoryName: 'ZK,HL,ZX',
    },
    {
        reservoirName: '德基水庫',
        factoryName: 'ZK,HL,ZX',
    },
    {
        reservoirName: '寶山水庫',
        factoryName: 'HS1,3',
    },
    {
        reservoirName: '寶山第二水庫',
        factoryName: 'HS1,3',
    },
];

const parseFactoryCodes = (rawFactoryName) => {
    const normalized = String(rawFactoryName || '').replace(/，/g, ',').trim();
    if (!normalized) return [];

    // 例如 HS1,3 視為單一代碼，不拆分
    if (/^[A-Za-z]+\d+(,\d+)+$/.test(normalized)) {
        return [normalized.toUpperCase()];
    }

    return [...new Set(
        normalized
            .split(',')
            .map((token) => token.trim())
            .filter(Boolean)
            .map((token) => token.toUpperCase())
    )];
};

const normalizeFactoryGroup = (rawFactoryName) => {
    const normalized = String(rawFactoryName || '')
        .replace(/，/g, ',')
        .split(',')
        .map((token) => token.trim())
        .filter(Boolean)
        .map((token) => token.toUpperCase())
        .join(',');
    return normalized;
};

const ReservoirFactoryMappingForm = () => {
    const [form] = Form.useForm();
    const [mappings, setMappings] = useState(INITIAL_MAPPINGS);
    const { data: stations } = useReservoirStation();
    const { data: realTimeInfos } = useReservoirRealTimeInfo();
    const { data: droughtAlert } = useDroughtAlert();

    const reservoirOptions = useMemo(() => {
        const fromApi = Array.isArray(stations)
            ? stations
                .map((item) => item.stationName ?? item.StationName)
                .filter(Boolean)
            : [];
        const fromMappings = mappings.map((item) => item.reservoirName).filter(Boolean);

        return [...new Set([...fromApi, ...fromMappings, ...DEFAULT_RESERVOIR_OPTIONS])]
            .sort((a, b) => a.localeCompare(b, 'zh-Hant'));
    }, [stations, mappings]);

    const mappedReservoirSet = useMemo(
        () => new Set(mappings.map((item) => item.reservoirName)),
        [mappings]
    );

    const reservoirCityMap = useMemo(() => {
        const stationArr = Array.isArray(stations) ? stations : [];
        const map = new Map();
        stationArr.forEach((item) => {
            const reservoirName = item.stationName ?? item.StationName;
            const cityCode = String(item.cityCode ?? item.CityCode ?? '');
            if (!reservoirName) return;
            const cityName = CITY_CODE_TO_NAME[cityCode] ?? cityCode ?? '-';
            map.set(reservoirName, cityName || '-');
        });
        return map;
    }, [stations]);

    const reservoirStatusMap = useMemo(() => {
        const map = new Map();
        const stationArr = Array.isArray(stations) ? stations : [];
        const realTimeArr = Array.isArray(realTimeInfos) ? realTimeInfos : [];
        const stationNoToName = new Map(
            stationArr.map((item) => [item.stationNo ?? item.StationNo, item.stationName ?? item.StationName])
        );

        realTimeArr.forEach((item) => {
            const stationNo = item.stationNo ?? item.StationNo;
            const reservoirName = stationNoToName.get(stationNo);
            if (!reservoirName) return;

            const volumeNumber = item.effectiveStorage ?? item.EffectiveStorage;
            const percentNumber = item.percentageOfStorage ?? item.PercentageOfStorage;

            map.set(reservoirName, {
                volumeNumber: typeof volumeNumber === 'number' ? volumeNumber : null,
                volumeText: typeof volumeNumber === 'number' ? Math.round(volumeNumber).toLocaleString() : '-',
                percentText: typeof percentNumber === 'number' ? `${percentNumber.toFixed(2)}%` : '-',
            });
        });

        return map;
    }, [stations, realTimeInfos]);

    const waterWarningSummary = useMemo(() => {
        const severity = droughtAlert?.info?.severity;
        const areaCount = (droughtAlert?.info?.area || []).length;
        const severityMap = {
            Minor: '水情提醒',
            Moderate: '減壓供水',
            Severe: '減量供水',
            Extreme: '分區供水或定點供水',
        };
        return {
            text: severityMap[severity] || '目前無水情燈號警戒',
            areaCount,
        };
    }, [droughtAlert]);

    const regionWarningMap = useMemo(() => {
        const severity = droughtAlert?.info?.severity;
        const meta = WARNING_SEVERITY_META[severity];
        const map = new Map();
        const areas = droughtAlert?.info?.area || [];
        areas.forEach((a) => {
            const key = normalizeRegion(a?.areaDesc);
            if (!key) return;
            map.set(key, meta || { text: '無燈號', color: 'default' });
        });
        return map;
    }, [droughtAlert]);

    const factoryAggregationRows = useMemo(() => {
        const bucket = new Map();

        mappings.forEach((item) => {
            const factoryGroup = normalizeFactoryGroup(item.factoryName);
            if (!factoryGroup) return;

            if (!bucket.has(factoryGroup)) {
                bucket.set(factoryGroup, {
                    factoryGroup,
                    reservoirs: [],
                    totalVolume: 0,
                    hasUnknownVolume: false,
                });
            }

            const row = bucket.get(factoryGroup);
            if (!row.reservoirs.includes(item.reservoirName)) {
                row.reservoirs.push(item.reservoirName);
            }

            const status = reservoirStatusMap.get(item.reservoirName);
            if (status?.volumeNumber != null) {
                row.totalVolume += status.volumeNumber;
            } else {
                row.hasUnknownVolume = true;
            }
        });

        const groupedRows = Array.from(bucket.values())
            .map((row) => ({
                ...row,
                mergedVolumeText: `${Math.round(row.totalVolume).toLocaleString()}${row.hasUnknownVolume ? ' (含未回傳資料)' : ''}`,
                reservoirDetails: [...row.reservoirs]
                    .map((reservoirName) => ({
                        reservoirName,
                        regionText: reservoirCityMap.get(reservoirName) ?? '-',
                    }))
                    .sort((a, b) => {
                        const regionCompare = a.regionText.localeCompare(b.regionText, 'zh-Hant');
                        if (regionCompare !== 0) return regionCompare;
                        return a.reservoirName.localeCompare(b.reservoirName, 'zh-Hant');
                    }),
            }))
            .sort((a, b) => a.factoryGroup.localeCompare(b.factoryGroup, 'zh-Hant'));

        return groupedRows.flatMap((group) =>
            group.reservoirDetails.map((detail, index) => {
                const regionSize = group.reservoirDetails.filter((d) => d.regionText === detail.regionText).length;
                const rowIndexInRegion = group.reservoirDetails
                    .slice(0, index)
                    .filter((d) => d.regionText === detail.regionText).length;

                return {
                    key: `${group.factoryGroup}-${detail.regionText}-${detail.reservoirName}`,
                    factoryGroup: group.factoryGroup,
                    regionText: detail.regionText,
                    mergedVolumeText: group.mergedVolumeText,
                    totalVolume: group.totalVolume,
                    reservoirName: detail.reservoirName,
                    groupSize: group.reservoirDetails.length,
                    rowIndexInGroup: index,
                    regionSize,
                    rowIndexInRegion,
                };
            })
        );
    }, [mappings, reservoirStatusMap, reservoirCityMap]);

    const handleSubmit = (values) => {
        setMappings((prev) => {
            const existed = prev.some((item) => item.reservoirName === values.reservoirName);
            if (existed) {
                return prev.map((item) =>
                    item.reservoirName === values.reservoirName ? { ...values } : item
                );
            }
            return [...prev, values];
        });

        message.success('已更新水庫對應工廠位置');
        form.resetFields();
    };

    const handleEdit = (record) => {
        form.setFieldsValue(record);
    };

    const handleDelete = (reservoirName) => {
        setMappings((prev) => prev.filter((item) => item.reservoirName !== reservoirName));
        message.success('已刪除對應設定');
    };

    const columns = [
        {
            title: '水庫',
            dataIndex: 'reservoirName',
            key: 'reservoirName',
            width: 150,
            render: (text) => <Tag color="blue">{text}</Tag>,
        },
        {
            title: '廠區',
            dataIndex: 'factoryName',
            key: 'factoryName',
            width: 180,
            sorter: (a, b) => a.factoryName.localeCompare(b.factoryName, 'zh-Hant'),
            sortDirections: ['ascend', 'descend'],
        },
        {
            title: '目前蓄水量',
            key: 'currentVolume',
            width: 140,
            render: (_, record) => reservoirStatusMap.get(record.reservoirName)?.volumeText ?? '-',
        },
        {
            title: '蓄水率',
            key: 'currentPercent',
            width: 110,
            render: (_, record) => reservoirStatusMap.get(record.reservoirName)?.percentText ?? '-',
        },
        {
            title: '操作',
            key: 'action',
            width: 140,
            render: (_, record) => (
                <Space>
                    <Button size="small" onClick={() => handleEdit(record)}>
                        編輯
                    </Button>
                    <Button size="small" danger onClick={() => handleDelete(record.reservoirName)}>
                        刪除
                    </Button>
                </Space>
            ),
        },
    ];

    const aggregationColumns = [
        {
            title: '區域',
            dataIndex: 'regionText',
            key: 'regionText',
            width: 140,
            onCell: (record) => ({
                rowSpan: record.rowIndexInRegion === 0 ? record.regionSize : 0,
            }),
        },
        {
            title: '水庫',
            dataIndex: 'reservoirName',
            key: 'reservoirName',
            width: 120,
            render: (text) => <Tag color="blue">{text}</Tag>,
        },
        {
            title: '廠區',
            dataIndex: 'factoryGroup',
            key: 'factoryGroup',
            width: 120,
            onCell: (record) => ({
                rowSpan: record.rowIndexInGroup === 0 ? record.groupSize : 0,
            }),
            render: (text) => {
                const codes = parseFactoryCodes(text);
                return (
                    <Space size={[4, 4]} wrap>
                        {codes.map((code) => (
                            <Tag key={`${text}-${code}`} color="cyan">
                                {code}
                            </Tag>
                        ))}
                    </Space>
                );
            },
        },
        {
            title: '合併蓄水量(萬立方公尺)',
            dataIndex: 'mergedVolumeText',
            key: 'mergedVolumeText',
            width: 220,
            onCell: (record) => ({
                rowSpan: record.rowIndexInGroup === 0 ? record.groupSize : 0,
            }),
        },
        {
            title: '燈號',
            key: 'warningStatus',
            width: 140,
            onCell: (record) => ({
                rowSpan: record.rowIndexInRegion === 0 ? record.regionSize : 0,
            }),
            render: (_, record) => {
                const warning = regionWarningMap.get(normalizeRegion(record.regionText))
                    || { text: '無燈號', color: 'default' };
                return <Tag color={warning.color}>{warning.text}</Tag>;
            },
        },
    ];

    return (
        <Card className="reservoir-factory-form-panel" bordered={false}>
            <Typography.Title level={5} style={{ marginTop: 0, marginBottom: 8 }}>
                水庫對應工廠位置設定
            </Typography.Title>
            <Typography.Paragraph type="secondary" style={{ marginBottom: 16 }}>
                總覽會顯示水庫對應工廠、目前蓄水量與水情燈號，若同一廠區對應多個水庫，將自動合併蓄水量。
            </Typography.Paragraph>
{/* 
            <Space size={12} wrap style={{ marginBottom: 12 }}>
                <Tag color="gold">水情燈號現況：{waterWarningSummary.text}</Tag>
                <Tag color="purple">影響區域數：{waterWarningSummary.areaCount}</Tag>
            </Space> */}

            <Form
                form={form}
                layout="vertical"
                className="reservoir-factory-form"
                onFinish={handleSubmit}
            >
                <Form.Item
                    label="水庫"
                    name="reservoirName"
                    rules={[{ required: true, message: '請選擇水庫' }]}
                >
                    <Select
                        showSearch
                        placeholder="請選擇水庫"
                        options={reservoirOptions.map((name) => ({ label: name, value: name }))}
                        optionFilterProp="label"
                        filterOption={(input, option) =>
                            (option?.label ?? '').toLowerCase().includes(input.toLowerCase())
                        }
                    />
                </Form.Item>

                <Form.Item
                    label="工廠名稱"
                    name="factoryName"
                    rules={[{ required: true, message: '請輸入工廠名稱' }]}
                >
                    <Input placeholder="例如：台南精密加工廠" />
                </Form.Item>

                <Space>
                    <Button type="primary" htmlType="submit">
                        儲存對應
                    </Button>
                    <Button onClick={() => form.resetFields()}>清除輸入</Button>
                </Space>
            </Form>

            <div style={{ marginTop: 18 }}>
                <Typography.Text type="secondary">
                    已設定 {mappedReservoirSet.size} / {reservoirOptions.length} 座水庫
                </Typography.Text>
            </div>

            <Table
                style={{ marginTop: 10 }}
                rowKey="reservoirName"
                dataSource={mappings}
                columns={columns}
                pagination={false}
                size="small"
                scroll={{ x: 760 }}
            />

            <Typography.Title level={5} style={{ marginTop: 18, marginBottom: 10 }}>
                廠區合併蓄水量
            </Typography.Title>

            <Table
                rowKey="key"
                dataSource={factoryAggregationRows}
                columns={aggregationColumns}
                pagination={false}
                size="small"
                locale={{ emptyText: '尚無可彙整資料' }}
                scroll={{ x: 760 }}
            />
        </Card>
    );
};

export default ReservoirFactoryMappingForm;
