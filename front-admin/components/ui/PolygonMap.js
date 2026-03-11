'use client';

import { useEffect, useRef } from 'react';
import 'leaflet/dist/leaflet.css';
import 'leaflet-draw/dist/leaflet.draw.css';

function wktToLatLngs(wkt) {
  if (!wkt) return [];
  const match = wkt.match(/POLYGON\s*\(\((.+)\)\)/i);
  if (!match) return [];
  return match[1].split(',').map((pair) => {
    const [lng, lat] = pair.trim().split(/\s+/).map(Number);
    return [lat, lng];
  });
}

function latLngsToWkt(latlngs) {
  if (!latlngs || latlngs.length === 0) return '';
  const coords = [...latlngs, latlngs[0]];
  const pairs = coords.map((ll) => `${ll.lng} ${ll.lat}`);
  return `POLYGON((${pairs.join(', ')}))`;
}

export default function PolygonMap({ lat, lng, polygonWkt, onChange }) {
  const containerRef = useRef(null);
  const mapRef = useRef(null);
  const markerRef = useRef(null);
  const drawnLayersRef = useRef(null);
  const onChangeRef = useRef(onChange);
  onChangeRef.current = onChange;

  useEffect(() => {
    if (!containerRef.current || mapRef.current) return;

    let L;
    try {
      L = require('leaflet');
      require('leaflet-draw');
    } catch {
      return;
    }

    // Fix default icon paths
    delete L.Icon.Default.prototype._getIconUrl;
    L.Icon.Default.mergeOptions({
      iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon-2x.png',
      iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon.png',
      shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-shadow.png',
    });

    const centerLat = lat || 41.015;
    const centerLng = lng || 28.979;

    const map = L.map(containerRef.current).setView([centerLat, centerLng], 13);
    mapRef.current = map;

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap',
    }).addTo(map);

    const marker = L.marker([centerLat, centerLng], { draggable: true }).addTo(map);
    markerRef.current = marker;
    marker.bindPopup('Restoran konumu — sürükleyerek değiştir').openPopup();

    function getCurrentPolygonWkt() {
      const layers = drawnLayersRef.current?.getLayers() || [];
      if (layers.length === 0) return '';
      return latLngsToWkt(layers[0].getLatLngs()[0]);
    }

    marker.on('dragend', () => {
      const pos = marker.getLatLng();
      onChangeRef.current?.({ lat: pos.lat, lng: pos.lng, polygonWkt: getCurrentPolygonWkt() });
    });

    map.on('click', (e) => {
      marker.setLatLng(e.latlng);
      onChangeRef.current?.({ lat: e.latlng.lat, lng: e.latlng.lng, polygonWkt: getCurrentPolygonWkt() });
    });

    const drawnLayers = new L.FeatureGroup();
    drawnLayersRef.current = drawnLayers;
    map.addLayer(drawnLayers);

    const existing = wktToLatLngs(polygonWkt);
    if (existing.length > 0) {
      drawnLayers.addLayer(L.polygon(existing, { color: '#f97316', fillOpacity: 0.15 }));
    }

    const drawControl = new L.Control.Draw({
      edit: { featureGroup: drawnLayers },
      draw: {
        polygon: { shapeOptions: { color: '#f97316', fillOpacity: 0.15 } },
        polyline: false, rectangle: false, circle: false, circlemarker: false, marker: false,
      },
    });
    map.addControl(drawControl);

    map.on(L.Draw.Event.CREATED, (e) => {
      drawnLayers.clearLayers();
      drawnLayers.addLayer(e.layer);
      const pos = markerRef.current.getLatLng();
      onChangeRef.current?.({ lat: pos.lat, lng: pos.lng, polygonWkt: latLngsToWkt(e.layer.getLatLngs()[0]) });
    });

    map.on(L.Draw.Event.EDITED, () => {
      const pos = markerRef.current.getLatLng();
      onChangeRef.current?.({ lat: pos.lat, lng: pos.lng, polygonWkt: getCurrentPolygonWkt() });
    });

    map.on(L.Draw.Event.DELETED, () => {
      const pos = markerRef.current.getLatLng();
      onChangeRef.current?.({ lat: pos.lat, lng: pos.lng, polygonWkt: '' });
    });

    return () => {
      map.remove();
      mapRef.current = null;
    };
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (!mapRef.current || !markerRef.current || !lat || !lng) return;
    markerRef.current.setLatLng([lat, lng]);
    mapRef.current.setView([lat, lng], mapRef.current.getZoom());
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [lat, lng]);

  return (
    <div
      ref={containerRef}
      style={{ height: '400px', width: '100%', borderRadius: '8px', zIndex: 0 }}
    />
  );
}
