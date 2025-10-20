import L from 'leaflet';
import { useEffect, useState } from 'react';
import { MapContainer, TileLayer, Marker, useMapEvents, useMap } from 'react-leaflet';

const markerIcon = L.icon({
  iconSize: [24, 41],
  iconAnchor: [10, 41],
  popupAnchor: [2, -40],
  iconUrl: 'https://unpkg.com/leaflet@1.6/dist/images/marker-icon.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.6/dist/images/marker-shadow.png',
});

L.Marker.prototype.options.icon = markerIcon;

const CustomMarker = ({ handleChange, setFieldValue, disabled }) => {
  const map = useMapEvents({
    click: (e) => {
      if (!disabled) {
        const { lat, lng } = e.latlng;
        handleChange(lat, lng);
        setFieldValue([lat, lng]);
      }
    },
  });
  return null;
};

const ChangeMapCenter = ( {center} ) => {
  const map = useMap()
  map.flyTo(center)
  return <></>
}

const ComponentResize = () => {
  const map = useMap()

  setTimeout(() => {
      map.invalidateSize()
  }, 0)

  return null
}

function Map(props) {
  const [location, setLocation] = useState(null);
  const [ positionCenter, setPositionCenter ] = useState([47.1000447, 104.0844727])

  useEffect(() => {
    if (props.value) {
      if (typeof props.value == 'string') {
        setLocation(
          Array.isArray(JSON.parse(props.value))
            ? JSON.parse(props.value)
            : null
        );
        props.onChange(JSON.parse(props.value));
      } else {
        props.onChange(props.value);
        setLocation(props.value);
      }
    }
  }, [props.value]);

  const handleChangeLocation = (lat, lng) => {
    setLocation([parseFloat(lat).toFixed(7), parseFloat(lng).toFixed(7)])
  }

  return (
    <div>
      <MapContainer
        center={positionCenter}
        zoom={5}
        scrollWheelZoom={true}
        doubleClickZoom={false}
        {...props}
        className={`h-[400px] rounded-lg  z-10 ${props.className}`}
      >
        <TileLayer 
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        <CustomMarker
          handleChange={handleChangeLocation}
          setFieldValue={props.onChange}
          disabled={props.disabled}
        />
        <ComponentResize/>
        <ChangeMapCenter center={location ? location : positionCenter}/> 
        {location && <Marker position={location} />}
        <div
          className={`${
            props.disabled ? 'block' : 'hidden'
          } absolute inset-0 z-[400] bg-black bg-opacity-30`}
        ></div>
      </MapContainer>
    </div>
  );
}
export default Map;
